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
    public partial class frmRetailStockCounting1 : Form
    {
        SqlConnection conn = new SqlConnection();
        public IPosTransaction pos { get; set; }
        Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations objBlank = new Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations();
        [Import]
        private IApplication application;

        DataTable skuItem = new DataTable();
        DataTable dtSku = new DataTable();
        DataTable dtImportSKU = new DataTable();
        bool bIsImport = false;



        public frmRetailStockCounting1()
        {
            InitializeComponent();
        }

        public frmRetailStockCounting1(IPosTransaction posTransaction, IApplication Application)
        {
            InitializeComponent();
            pos = posTransaction;
            application = Application;
            loadVoucher();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void loadVoucher()
        {
            string sSQl = "select VOUCHERNUM from RETAILSTOCKCOUNTHEADER where COUNTINGCLOSE=0";

            cmbVoucherSelection.DataSource = null;
            cmbVoucherSelection.Items.Add(" ");
            cmbVoucherSelection.DataSource = objBlank.NIM_LoadCombo("", "", "", sSQl);
            cmbVoucherSelection.DisplayMember = "VOUCHERNUM";
            cmbVoucherSelection.ValueMember = "VOUCHERNUM";
        }

        private void btnCountedBy_Click(object sender, EventArgs e)
        {
            LSRetailPosis.POSProcesses.frmSalesPerson dialog6 = new LSRetailPosis.POSProcesses.frmSalesPerson();
            dialog6.ShowDialog();
            if (!string.IsNullOrEmpty(dialog6.SelectedEmployeeId))
            {
                txtCountedBy.Tag = dialog6.SelectedEmployeeId;
                txtCountedBy.Text = dialog6.SelectEmployeeName;
            }
        }

        public bool isValiedSku(SqlConnection conn, string _productSku)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            string commandText = string.Empty;

            commandText = "SELECT CAST(SKUNUMBER AS NVARCHAR (30)) AS SKUNUMBER" +
                          " FROM SKUTable_Posted WHERE " +
                          " SKUNUMBER IN (SELECT SKUNUMBER FROM SKUTableTrans WHERE ISAVAILABLE=1)" +
                          " and SkuNumber in (select itemid from ASSORTEDINVENTITEMS ) " +
                          " AND SKUNUMBER='" + _productSku + "'";

            SqlCommand command = new SqlCommand(commandText, conn);
            command.CommandTimeout = 0;

            string sku = Convert.ToString(command.ExecuteScalar());

            if (sku != string.Empty)
                return true;
            else
                return false;

        }

        private void txtSKUNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(txtCountedBy.Tag)))
            {
                if (e.KeyValue == 13)
                {
                    if (ValidateItem())
                    {
                        AddItemIntoGrid();
                    }
                }
            }
            else
            {
                MessageBox.Show("Counted by person selection is required");
                btnCountedBy.Focus();
            }
        }

        private void AddItemIntoGrid()
        {
            string sStoreId = ApplicationSettings.Terminal.StoreId;

            getSkuDetails(Convert.ToString(txtSKUNo.Text));
            txtSKUNo.Text = string.Empty; // added on 17.08.2013
        }

        private void getSkuDetails(string _productSku)
        {
            int iNoSku = 0;
            int iNoOfSetOf = 0;
            decimal dTotQty = 0;

            conn = application.Settings.Database.Connection;
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            string commandText = string.Empty;
            commandText = " SELECT top 1 sp.SKUNUMBER as SKUNumber,f.NAME Name, " +//isnull(I.SetOf,0) as PCS,CONVERT(DECIMAL(10,3),QTY) as QTY
                           " CASE WHEN I.METALTYPE =14 or  I.METALTYPE =18 THEN  CAST(ISNULL(sp.QTY,0) AS NUMERIC (28,0))   ELSE CAST(ISNULL(sp.PDSCWQTY,0) AS NUMERIC (28,0))  END AS PCS," +
                           " CAST(ISNULL(sp.GrossWeight,0) AS NUMERIC (28,3)) as QTY ,'' SalesMan" +
                           " FROM SKUTable_Posted sp" +
                           " LEFT JOIN INVENTTABLE I ON sp.SKUNUMBER = I.ITEMID " +
                           " LEFT OUTER JOIN ECORESPRODUCT E ON i.PRODUCT = E.RECID" +
                           " LEFT OUTER JOIN ECORESPRODUCTTRANSLATION F ON E.RECID = F.PRODUCT and f.LANGUAGEID='en-us'" +
                           " WHERE sp.SKUNUMBER='" + _productSku + "'";

            SqlCommand command = new SqlCommand(commandText, conn);
            command.CommandTimeout = 0;
            SqlDataAdapter adapter = new SqlDataAdapter(commandText, conn);
            adapter.Fill(dtSku);

            if (conn.State == ConnectionState.Open)
                conn.Close();

            if (dtSku != null && dtSku.Rows.Count > 0)
            {
                string s = _productSku;
                //DataColumn[] columns = new DataColumn[1];
                //columns[0] = dtSku.Columns["SKUNUMBER"];
                //dtSku.PrimaryKey = columns;

                bool isExist = IsExistInLocal(_productSku);

                if (!isExist)
                {
                    AddItem(_productSku, ref iNoSku, ref iNoOfSetOf, ref dTotQty, ref commandText, command, adapter);
                }

                foreach (DataRow d in dtSku.Rows)
                {
                    d["SalesMan"] = Convert.ToString(txtCountedBy.Tag);
                }
                dtSku.AcceptChanges();

                skuItem = dtSku;
                grItems.DataSource = dtSku.DefaultView;

                CountTotalSKU(ref iNoSku, ref iNoOfSetOf, ref dTotQty);
            }
            else
            {
                AddItem(_productSku, ref iNoSku, ref iNoOfSetOf, ref dTotQty, ref commandText, command, adapter);

                CountTotalSKU(ref iNoSku, ref iNoOfSetOf, ref dTotQty);
            }
        }

        private void CountTotalSKU(ref int iNoSku, ref int iNoOfSetOf, ref decimal dTotQty)
        {
            //Start : added on 26/05/2014
            foreach (DataRow dr in dtSku.Rows)
            {
                iNoSku = iNoSku + 1;
                iNoOfSetOf = iNoOfSetOf + Convert.ToInt32(dr[2]);
                dTotQty = dTotQty + Convert.ToDecimal(dr[3]);
            }

            lblTotNoOfSKU.Text = Convert.ToString(iNoSku);
            lblTotQty.Text = Convert.ToString(dTotQty);
            //End : added on 26/05/2014
        }

        private void AddItem(string _productSku, ref int iNoSku, ref int iNoOfSetOf, ref decimal dTotQty, ref string commandText, SqlCommand command, SqlDataAdapter adapter)
        {
            if (dtSku != null && dtSku.Rows.Count == 0)
            {
                dtSku.Rows.Add();
                dtSku.Rows[0]["SKUNumber"] = Convert.ToString(_productSku);
                dtSku.Rows[0]["Name"] = "";
                dtSku.Rows[0]["PCS"] = 1;
                dtSku.Rows[0]["QTY"] = 1;
                dtSku.Rows[0]["SalesMan"] = Convert.ToString(txtCountedBy.Tag);
            }
            else if (dtSku != null && dtSku.Rows.Count > 0)
            {
                var row = dtSku.NewRow();
                row["SKUNumber"] = Convert.ToString(_productSku);
                row["Name"] = "";
                row["PCS"] = 1;
                row["QTY"] = 1;
                row["SalesMan"] = Convert.ToString(txtCountedBy.Tag);
                dtSku.Rows.Add(row);
            }

            dtSku.AcceptChanges();

            skuItem = dtSku;
            grItems.DataSource = dtSku.DefaultView;


        }

        private bool IsExistInLocal(string sSku)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;
            string commandText = "";

            if (!string.IsNullOrEmpty(sSku))
            {
                commandText = "SELECT top 1 isnull(SKUNUMBER,'') SKUNUMBER" +
                                 " from SKUTable_Posted where  SKUNUMBER ='" + sSku + "'";
            }

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (!string.IsNullOrEmpty(sResult))
                return true;
            else
                return false;
        }

        private bool IsExistInInventtable(string sSku)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;
            string commandText = "";

            if (!string.IsNullOrEmpty(sSku))
            {
                commandText = "select RECID from INVENTTABLE where ITEMID ='" + sSku + "'";
            }

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command1 = new SqlCommand(commandText.ToString(), conn);
            command1.CommandTimeout = 0;
            string sResult = Convert.ToString(command1.ExecuteScalar());

            if (!string.IsNullOrEmpty(sResult))
                return true;
            else
                return false;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(txtCountedBy.Tag)))
            {
                if (!string.IsNullOrEmpty(Convert.ToString(txtSKUNo.Text)))
                {
                    AddItemIntoGrid();
                }
                else
                {
                    MessageBox.Show("Enter a SKU.");
                    txtSKUNo.Focus();
                }
            }
            else
            {
                MessageBox.Show("Counted by person selection is required");
                btnCountedBy.Focus();
            }
        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(cmbVoucherSelection.Text)))
            {
                if (dtSku != null && dtSku.Rows.Count > 0)
                {
                    SaveCountingDetails();
                    ClearControl();
                }
                else
                {
                    MessageBox.Show("Atleast one item should be count");
                    cmbVoucherSelection.Focus();
                }
            }
            else
            {
                MessageBox.Show("voucher selection is required");
                cmbVoucherSelection.Focus();
            }
        }

        private void SaveCountingDetails()
        {
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlTransaction transaction = null;
            try
            {
                transaction = connection.BeginTransaction();

                if (dtSku != null && dtSku.Rows.Count > 0)
                {
                    string commandText = " INSERT INTO [RETAILSTOCKCOUNTDETAILS] " +
                                   " ([VOUCHERNUM],[ITEMID] " +
                                   " ,[STOREID],[TERMINALID],[PCS],[QTY],[SYSPCS]" +
                                   " ,[SYSQTY],[PRODUCTTYPE],[ARTICLE],[COUNTEDBY]) " +
                                   "  VALUES " +
                                   " (@VOUCHERNUM,@ITEMID,@STOREID,@TERMINALID,@PCS," +
                                   " @QTY,@SYSPCS,@SYSQTY,@PRODUCTTYPE,@ARTICLE,@COUNTEDBY)";

                    for (int ItemCount = 0; ItemCount < dtSku.Rows.Count; ItemCount++)
                    {
                        using (SqlCommand cmdItem = new SqlCommand(commandText, connection, transaction))
                        {
                            cmdItem.CommandTimeout = 0;
                            cmdItem.Parameters.Add("@VOUCHERNUM", SqlDbType.VarChar, 20).Value = Convert.ToString(cmbVoucherSelection.Text);
                            cmdItem.Parameters.Add("@ITEMID", SqlDbType.VarChar, 20).Value = Convert.ToString(dtSku.Rows[ItemCount]["SKUNUMBER"]);
                            cmdItem.Parameters.Add("@STOREID", SqlDbType.VarChar, 20).Value = ApplicationSettings.Terminal.StoreId;
                            cmdItem.Parameters.Add("@TERMINALID", SqlDbType.VarChar, 20).Value = ApplicationSettings.Terminal.TerminalId;
                            cmdItem.Parameters.Add("@PCS", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSku.Rows[ItemCount]["PCS"]);
                            cmdItem.Parameters.Add("@QTY", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSku.Rows[ItemCount]["QTY"]);
                            cmdItem.Parameters.Add("@SYSPCS", SqlDbType.Decimal).Value = 0;
                            cmdItem.Parameters.Add("@SYSQTY", SqlDbType.Decimal).Value = 0;
                            cmdItem.Parameters.Add("@PRODUCTTYPE", SqlDbType.VarChar, 20).Value = txtProdType.Text.Trim();
                            cmdItem.Parameters.Add("@ARTICLE", SqlDbType.VarChar, 20).Value = txtArticle.Text.Trim();
                            cmdItem.Parameters.Add("@COUNTEDBY", SqlDbType.VarChar, 20).Value = Convert.ToString(dtSku.Rows[ItemCount]["SalesMan"]);

                            cmdItem.ExecuteNonQuery();
                            cmdItem.Dispose();
                        }
                    }
                }
                transaction.Commit();
                transaction.Dispose();
            }
            #region Exception
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
                MessageBox.Show("Stock counting is successfully complete.");
            }
            #endregion
        }

        private void ClearControl()
        {
            grItems.DataSource = null;
            dtSku.Clear();
            lblTotNoOfSKU.Text = "";
            lblTotQty.Text = "";
        }

        private void cmbVoucherSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cmbVoucherSelection.Text))
                GetVouInfo(cmbVoucherSelection.Text);
            else
            {
                txtProdType.Text = "";
                txtArticle.Text = "";
            }
        }

        private void GetVouInfo(string sVouNum)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string commandText = " SELECT  TOP (1) isnull(PRODUCTTYPE,'') PRODUCTTYPE,isnull(ARTICLE,'') ARTICLE" +
                            " FROM   RETAILSTOCKCOUNTHEADER  " +
                            " WHERE VOUCHERNUM='" + sVouNum + "'";


            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    txtProdType.Text = Convert.ToString(reader.GetValue(0));
                    txtArticle.Text = Convert.ToString(reader.GetValue(1));
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();

        }

        private void txtSKUNo_Validating(object sender, CancelEventArgs e)
        {
            ValidateItem();
        }

        private bool ValidateItem()
        {
            bool bResult = true;
            if (!string.IsNullOrEmpty(txtSKUNo.Text))
            {
                bool isExist = IsExistInInventtable(txtSKUNo.Text);
                string sProdType = string.Empty;
                string sArticle = string.Empty;

                if (isExist)
                {
                    GetItemInfo(txtSKUNo.Text, ref sProdType, ref sArticle);
                    if (Convert.ToString(txtProdType.Text) != sProdType ||
                        Convert.ToString(txtArticle.Text) != sArticle)
                    {
                        MessageBox.Show("Invalid selected item");
                        txtSKUNo.Focus();
                        bResult = false;
                    }
                }
                else
                    MessageBox.Show("SKU is not in your stock.");
            }
            return bResult;
        }

        private void GetItemInfo(string sItemId, ref string sProdTye, ref string sArtCode)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string commandText = " SELECT  TOP (1) isnull(PRODUCTTYPECODE,'') PRODUCTTYPE,isnull(ARTICLE_CODE,'') ARTICLE" +
                            " FROM   INVENTTABLE  " +
                            " WHERE ITEMID='" + sItemId + "'";


            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    sProdTye = Convert.ToString(reader.GetValue(0));
                    sArtCode = Convert.ToString(reader.GetValue(1));
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();

        }
    }
}
