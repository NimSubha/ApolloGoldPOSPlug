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
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmForeignDenomination : Form
    {
        [Import]
        private IApplication application;
        public bool IsEdit { get; set; }
        Random randUnique = new Random();
        string sUnique = string.Empty;
        int EditselectedIndex = 0;
        public IPosTransaction pos { get; set; }

        DataTable dtCurrencyInfo = new DataTable("dtCurrencyInfo");

        public frmForeignDenomination()
        {
            InitializeComponent();
        }

        public frmForeignDenomination(IPosTransaction posTransaction, IApplication Application)
        {
            InitializeComponent();
            pos = posTransaction;
            application = Application;
        }


        private void btnClearCurrency_Click(object sender, EventArgs e)
        {
            txtCurrency.Text = "";
            txtCurrency.Focus();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dCurrency = GetCurrencyCode();

                if (dCurrency != null && dCurrency.Rows.Count > 0)
                {
                    Dialog.WinFormsTouch.frmGenericSearch Osearch = new Dialog.WinFormsTouch.frmGenericSearch(dCurrency, null, "Currency");
                    Osearch.ShowDialog();
                    DataRow dr = Osearch.SelectedDataRow;

                    if (dr != null)
                    {
                        txtCurrency.Text = Convert.ToString(dr["Name"]);
                        txtCurrency.Tag = Convert.ToString(dr["Code"]);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private DataTable GetCurrencyCode()
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            DataTable dt = new DataTable();
            string commandText = "select CURRENCYCODE as Code,TXT as Name from CURRENCY ";
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            if (ValidateCurrency(Convert.ToString(txtCurrency.Tag)))
            {
                DataRow dr;
                if (IsEdit == false && dtCurrencyInfo != null && dtCurrencyInfo.Rows.Count == 0 && dtCurrencyInfo.Columns.Count == 0)
                {
                    dtCurrencyInfo.Columns.Add("UNIQUEID", typeof(string));
                    dtCurrencyInfo.Columns.Add("CURRENCY", typeof(string));
                    dtCurrencyInfo.Columns.Add("RATE", typeof(decimal));
                    dtCurrencyInfo.Columns.Add("QUANTITY", typeof(decimal));
                    dtCurrencyInfo.Columns.Add("AMOUNT", typeof(decimal));
                }

                if (IsEdit == false)
                {
                    dr = dtCurrencyInfo.NewRow();
                    dr["UNIQUEID"] = sUnique = Convert.ToString(randUnique.Next());
                    dr["CURRENCY"] = Convert.ToString(txtCurrency.Tag);

                    if (!string.IsNullOrEmpty(txtRate.Text.Trim()))
                        dr["RATE"] = decimal.Round(Convert.ToDecimal(txtRate.Text.Trim()), 4, MidpointRounding.AwayFromZero);
                    else
                        dr["RATE"] = 0m;

                    if (!string.IsNullOrEmpty(txtQuantity.Text.Trim()))
                        dr["QUANTITY"] = decimal.Round(Convert.ToDecimal(txtQuantity.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                    else
                        dr["QUANTITY"] = DBNull.Value;
                    if (!string.IsNullOrEmpty(txtTotal.Text.Trim()))
                        dr["AMOUNT"] = decimal.Round(Convert.ToDecimal(txtTotal.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                    else
                        dr["AMOUNT"] = DBNull.Value;

                    dtCurrencyInfo.Rows.Add(dr);
                    grItems.DataSource = dtCurrencyInfo.DefaultView;
                }

                if (IsEdit == true)
                {
                    DataRow EditRow = dtCurrencyInfo.Rows[EditselectedIndex];

                    //string query = " SELECT TOP(1) @CURRENCYCODE=CURRENCYCODE FROM CURRENCY WHERE CURRENCYCODE='" + Convert.ToString(txtCurrency.Tag) + "'";
                    //txtCurrency.Text = NIM_ReturnExecuteScalar(query);

                    if (!string.IsNullOrEmpty(txtCurrency.Text.Trim()))
                        EditRow["CURRENCY"] = Convert.ToString(txtCurrency.Tag);
                    else
                        EditRow["CURRENCY"] = "";

                    if (!string.IsNullOrEmpty(txtRate.Text.Trim()))
                        EditRow["RATE"] = decimal.Round(Convert.ToDecimal(txtRate.Text.Trim()), 4, MidpointRounding.AwayFromZero);
                    else
                        EditRow["RATE"] = 0m;

                    if (!string.IsNullOrEmpty(txtQuantity.Text.Trim()))
                        EditRow["QUANTITY"] = decimal.Round(Convert.ToDecimal(txtQuantity.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                    else
                        EditRow["QUANTITY"] = DBNull.Value;

                    if (!string.IsNullOrEmpty(txtTotal.Text.Trim()))
                        EditRow["AMOUNT"] = decimal.Round(Convert.ToDecimal(txtTotal.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                    else
                        EditRow["AMOUNT"] = DBNull.Value;

                    dtCurrencyInfo.AcceptChanges();

                    grItems.DataSource = dtCurrencyInfo.DefaultView;
                    IsEdit = false;
                }
                ClearControls();
            }
        }

        private void ClearControls()
        {
            txtCurrency.Text = "";
            txtRate.Text = "";
            txtQuantity.Text = "";
            txtTotal.Text = "";
        }

        private bool ValidateCurrency(string sCurrency)
        {
            string sOutPut = string.Empty;

            string query = " DECLARE @CURRENCYCODE NVARCHAR(20) " +
                            " SELECT TOP(1) @CURRENCYCODE=CURRENCYCODE FROM CURRENCY WHERE CURRENCYCODE='" + sCurrency + "'" +
                            " IF ISNULL(@CURRENCYCODE,'')='' " +
                                " BEGIN " +
                                    " SELECT 'False' " +
                                " END " +
                            " ELSE " +
                                " BEGIN " +
                                    " SELECT 'True' " +
                                " END ";


            sOutPut = NIM_ReturnExecuteScalar(query);

            return Convert.ToBoolean(sOutPut);
        }

       

        public string NIM_ReturnExecuteScalar(string query)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection myCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
            myCon.Open();

            try
            {
                if (myCon.State == ConnectionState.Closed)
                    myCon.Open();
                cmd = new SqlCommand(query, myCon);
                return Convert.ToString(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
            finally
            {
                if (myCon.State == ConnectionState.Open)
                    myCon.Close();
            }
        }

        private void txtRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void txtQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            int iTransfer_Details = 0;
            SqlTransaction transaction = null;
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                transaction = connection.BeginTransaction();
                string commandText = " INSERT INTO [CRWFOREIGNDENOMINATION]([CURRENCY],[RATE],[QUANTITY],[AMOUNT]," +
                                                " [STOREID],[TRANSDATE],[TERMINALID],STAFFID,[DATAAREAID])" +
                                                " VALUES(@CURRENCY  ,@RATE , @QUANTITY," +
                                                " @AMOUNT,@STOREID,@TRANSDATE," +
                                                " @TERMINALID,@STAFFID,@DATAAREAID) ";
               
                if (dtCurrencyInfo != null && dtCurrencyInfo.Rows.Count > 0)
                {
                    #region  DETAILS & Transaction
                   

                    for (int ItemCount = 0; ItemCount < dtCurrencyInfo.Rows.Count; ItemCount++)
                    {
                        SqlCommand cmdDetail = new SqlCommand(commandText, connection, transaction);

                        cmdDetail.Parameters.Add("@CURRENCY", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtCurrencyInfo.Rows[ItemCount]["CURRENCY"]);

                        if (string.IsNullOrEmpty(Convert.ToString(dtCurrencyInfo.Rows[ItemCount]["RATE"])))
                            cmdDetail.Parameters.Add("@RATE", SqlDbType.Decimal).Value = DBNull.Value;
                        else
                            cmdDetail.Parameters.Add("@RATE", SqlDbType.Decimal).Value = Convert.ToDecimal(dtCurrencyInfo.Rows[ItemCount]["RATE"]);

                        if (string.IsNullOrEmpty(Convert.ToString(dtCurrencyInfo.Rows[ItemCount]["QUANTITY"])))
                            cmdDetail.Parameters.Add("@QUANTITY", SqlDbType.Decimal).Value = DBNull.Value;
                        else
                            cmdDetail.Parameters.Add("@QUANTITY", SqlDbType.Decimal).Value = Convert.ToDecimal(dtCurrencyInfo.Rows[ItemCount]["QUANTITY"]);

                        if (string.IsNullOrEmpty(Convert.ToString(dtCurrencyInfo.Rows[ItemCount]["AMOUNT"])))
                            cmdDetail.Parameters.Add("@AMOUNT", SqlDbType.Decimal).Value = DBNull.Value;
                        else
                            cmdDetail.Parameters.Add("@AMOUNT", SqlDbType.Decimal).Value = Convert.ToDecimal(dtCurrencyInfo.Rows[ItemCount]["AMOUNT"]);

                        cmdDetail.Parameters.Add("@STOREID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.StoreId;
                        cmdDetail.Parameters.Add("@TRANSDATE", SqlDbType.DateTime).Value = DateTime.Today.ToShortDateString();

                        cmdDetail.Parameters.Add("@TERMINALID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.TerminalId;
                        cmdDetail.Parameters.Add("@STAFFID", SqlDbType.NVarChar, 10).Value = pos.OperatorId;
                        if (application != null)
                            cmdDetail.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = application.Settings.Database.DataAreaID;
                        else
                            cmdDetail.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;

                        cmdDetail.CommandTimeout = 0;
                        iTransfer_Details = cmdDetail.ExecuteNonQuery();
                        cmdDetail.Dispose();
                    }

                    #endregion
                }

                transaction.Commit();
                transaction.Dispose();

                if (iTransfer_Details != 0)
                {
                    MessageBox.Show("Save successfully");

                    dtCurrencyInfo.Clear();

                    grItems.DataSource = null;
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
                    connection.Close();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            IsEdit = false;
            if (dtCurrencyInfo != null && dtCurrencyInfo.Rows.Count > 0)
            {
                if (grdView.RowCount > 0)
                {
                    IsEdit = true;
                    EditselectedIndex = grdView.GetSelectedRows()[0];
                    DataRow theRowToSelect = dtCurrencyInfo.Rows[EditselectedIndex];
                    string query = " SELECT TOP(1) TXT FROM CURRENCY WHERE CURRENCYCODE='" + Convert.ToString(theRowToSelect["CURRENCY"]) + "'";
                    txtCurrency.Text = NIM_ReturnExecuteScalar(query);

                    txtCurrency.Tag = Convert.ToString(theRowToSelect["CURRENCY"]);
                    txtRate.Text = Convert.ToString(theRowToSelect["RATE"]);
                    txtTotal.Text = Convert.ToString(theRowToSelect["AMOUNT"]);
                    txtQuantity.Text = Convert.ToString(theRowToSelect["QUANTITY"]);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int DeleteSelectedIndex = 0;
            if (dtCurrencyInfo != null && dtCurrencyInfo.Rows.Count > 0)
            {
                if (grdView.RowCount > 0)
                {
                    DeleteSelectedIndex = grdView.GetSelectedRows()[0];
                    DataRow theRowToDelete = dtCurrencyInfo.Rows[DeleteSelectedIndex];

                    dtCurrencyInfo.Rows.Remove(theRowToDelete);
                    grItems.DataSource = dtCurrencyInfo.DefaultView;
                }
            }


            if (DeleteSelectedIndex == 0 && dtCurrencyInfo != null && dtCurrencyInfo.Rows.Count == 0)
            {
                grItems.DataSource = null;
                dtCurrencyInfo.Clear();
            }
            IsEdit = false;
            ClearControls();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearControls();
            grItems.DataSource = null;
            dtCurrencyInfo.Clear();
        }

        private void txtQuantity_Leave(object sender, EventArgs e)
        {
            decimal dRate = 0m;
            decimal dQty = 0m;
            decimal dTotVal = 0m;


            if (!string.IsNullOrEmpty(txtRate.Text))
                dRate = Convert.ToDecimal(txtRate.Text);

            if (!string.IsNullOrEmpty(txtQuantity.Text))
                dQty = Convert.ToDecimal(txtQuantity.Text);


            dTotVal = dRate * dQty;

            txtTotal.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(dTotVal), 4, MidpointRounding.AwayFromZero));
        }
    }
}
