using System;
using System.Data;
using System.Windows.Forms;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.Data.SqlClient;
using LSRetailPosis.Settings;


namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmCustomerFootfall : Form
    {
        public IPosTransaction pos { get; set; }
        [Import]
        private IApplication application;
        private bool bIsEdit = false;

        public frmCustomerFootfall(IPosTransaction posTransaction, IApplication Application)
        {
            InitializeComponent();
            pos = posTransaction;
            application = Application;
            btnEdit.Enabled = false;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            SqlTransaction transaction = null;
            int iCustFootFall = 0;
            string commandText = string.Empty;

            if (isValid())
            {
                #region Customer Footfall
                if (!bIsEdit)
                {
                    
                    commandText = " INSERT INTO [CUSTOMERFOOTFALL]([DATE],[NOOFEXISTCUST]," +
                                        " [NOOFNEWCUST],[NOOFHNICUST],[DATAAREAID],[STOREID],[TERMINALID]," +
                                        " [STAFFID])" +
                                        " VALUES(@DATE,@NOOFEXISTCUST,@NOOFNEWCUST,@NOOFHNICUST,@DATAAREAID," +
                                        " @STOREID,@TERMINALID,@STAFFID)";
                    
                }
                else
                {
                    commandText = " UPDATE [CUSTOMERFOOTFALL]" +
                                        " SET [DATE]=@DATE," +
                                        " [NOOFEXISTCUST]=@NOOFEXISTCUST," +
                                        " [NOOFNEWCUST]=@NOOFNEWCUST," +
                                        " [NOOFHNICUST]=@NOOFHNICUST," +
                                        " [DATAAREAID]=@DATAAREAID," +
                                        " [STOREID]=@STOREID," +
                                        " [TERMINALID]=@TERMINALID," +
                                        " [STAFFID]=@STAFFID " +
                                        " WHERE REPLICATIONCOUNTER= " + txtExistCust.Tag + "";
                }
                SqlConnection connection = new SqlConnection();
                try
                {
                    if (application != null)
                        connection = application.Settings.Database.Connection;
                    else
                        connection = ApplicationSettings.Database.LocalConnection;


                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    transaction = connection.BeginTransaction();

                    SqlCommand command = new SqlCommand(commandText, connection, transaction);
                    command.Parameters.Clear();
                    command.Parameters.Add("@DATE", SqlDbType.Date).Value = dtpDate.Value;
                    if (string.IsNullOrEmpty(txtExistCust.Text.Trim()))
                        command.Parameters.Add("@NOOFEXISTCUST", SqlDbType.Int).Value = 0;
                    else
                        command.Parameters.Add("@NOOFEXISTCUST", SqlDbType.Int).Value = Convert.ToInt32(txtExistCust.Text.Trim());

                    if (string.IsNullOrEmpty(txtNewCust.Text.Trim()))
                        command.Parameters.Add("@NOOFNEWCUST", SqlDbType.Int).Value = 0;
                    else
                        command.Parameters.Add("@NOOFNEWCUST", SqlDbType.Int).Value = Convert.ToInt32(txtNewCust.Text.Trim());

                    if (string.IsNullOrEmpty(txtHNICust.Text.Trim()))
                        command.Parameters.Add("@NOOFHNICUST", SqlDbType.Int).Value = 0;
                    else
                        command.Parameters.Add("@NOOFHNICUST", SqlDbType.Int).Value = Convert.ToInt32(txtHNICust.Text.Trim());


                    if (application != null)
                        command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = application.Settings.Database.DataAreaID;
                    else
                        command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;

                    command.Parameters.Add("@STOREID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.StoreId;
                    command.Parameters.Add("@TERMINALID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.TerminalId;
                    command.Parameters.Add("@STAFFID", SqlDbType.NVarChar, 10).Value = pos.OperatorId;
                    //=====================================
                    command.CommandTimeout = 0;
                    iCustFootFall = command.ExecuteNonQuery();

                    transaction.Commit();
                    command.Dispose();
                    transaction.Dispose();
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

                #endregion
                ClearControls();
            }
            
            if (bIsEdit)
            {
                ShowCustFootFallDetails();
                bIsEdit = false;
                btnEdit.Enabled = false;
            } 
        }

        #region ClearControls
        /// <summary>
        /// </summary>
        private void ClearControls()
        {
            txtNewCust.Text = string.Empty;
            txtExistCust.Text = string.Empty;
            txtHNICust.Text = string.Empty;
        }

        #endregion

        private void btnShowRec_Click(object sender, EventArgs e)
        {
            ShowCustFootFallDetails();
        }

        private void ShowCustFootFallDetails()
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;
            string storeId = LSRetailPosis.Settings.ApplicationSettings.Terminal.StoreId;
            DataTable dt = new DataTable();

            string sQuery = " SELECT CONVERT(VARCHAR(11),[DATE],103) as [ENTRYDATE]," +
                            " [NOOFEXISTCUST],[NOOFNEWCUST],[NOOFHNICUST],[REPLICATIONCOUNTER],"+
                            " ([NOOFEXISTCUST]+[NOOFNEWCUST]+[NOOFHNICUST]) AS TOTAL" +
                            " FROM   CUSTOMERFOOTFALL " +
                            " WHERE STOREID = '" + storeId + "'" +
                            " AND DATE BETWEEN '" + Convert.ToDateTime(dtpFromDate.Text).ToString("dd-MMM-yyyy") + "'" +
                            " AND '" + Convert.ToDateTime(dtpToDate.Text).ToString("dd-MMM-yyyy") + "'";

            using (SqlCommand command = new SqlCommand(sQuery, conn))
            {
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(dt);
            }

            if (dt != null && dt.Rows.Count > 0)
                grdCustFootFall.DataSource = dt;
            else
                grdCustFootFall.DataSource = null;
        }

        private void btnCLose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void grdCustFootFall_DoubleClick(object sender, EventArgs e)
        {
            EditGridRow(gridView);
            btnEdit.Enabled = true;
        }

        private void EditGridRow(DevExpress.XtraGrid.Views.Grid.GridView view)
        {
            if (view == null || view.SelectedRowsCount == 0) return;

            DataRow[] rows = new DataRow[view.SelectedRowsCount];
            for (int i = 0; i < view.SelectedRowsCount; i++)
                rows[i] = view.GetDataRow(view.GetSelectedRows()[i]);

            view.BeginSort();
            try
            {
                foreach (DataRow rn in rows)
                {
                    dtpDate.Text = Convert.ToDateTime(rn[0]).ToShortDateString();

                    txtExistCust.Text = Convert.ToString(rn[1]);
                    txtNewCust.Text = Convert.ToString(rn[2]);
                    txtHNICust.Text = Convert.ToString(rn[3]);
                    txtExistCust.Tag  = Convert.ToInt32(rn[4]);

                    bIsEdit = false;
                    dtpDate.Enabled = false;
                    txtExistCust.Enabled=false;
                    txtNewCust.Enabled  =false;
                    txtHNICust.Enabled  =false ;
                }
            }
            finally
            {
                view.EndSort();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            dtpDate.Enabled = true;
            txtExistCust.Enabled = true ;
            txtNewCust.Enabled = true;
            txtHNICust.Enabled = true;
            bIsEdit = true;
        }

        private void txtExistCust_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtNewCust_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtHNICust_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private bool  bIsValiedDateEntry(DateTime dDate)
        {
            bool bReturn = false;
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string commandText = " DECLARE @CUSTFOOTFALLSTARTDATE DATE" +
                                 " SELECT  @CUSTFOOTFALLSTARTDATE=CUSTFOOTFALLSTARTDATE FROM RETAILPARAMETERS" +
                                 " if('" + Convert.ToDateTime(dDate).ToString("dd-MMM-yyyy") + "'=@CUSTFOOTFALLSTARTDATE)" +
                                 " BEGIN" +
                                 "   SELECT 1 " +
                                 " END" +
                                 " ELSE If('" + Convert.ToDateTime(dDate.AddDays(-1)).ToString("dd-MMM-yyyy") + "' >= @CUSTFOOTFALLSTARTDATE)" +
                                 " BEGIN" +
                                 "   SELECT REPLICATIONCOUNTER FROM CUSTOMERFOOTFALL " +
                                 "   WHERE [DATE] = '" + Convert.ToDateTime(dDate.AddDays(-1)).ToString("dd-MMM-yyyy") + "' " +
                                 " END";
            
            SqlCommand command = new SqlCommand(commandText, connection);

            string sResult = Convert.ToString(command.ExecuteScalar());
            connection.Close();
            if (!string.IsNullOrEmpty(sResult))
            {
                bReturn = true;
            }
            return bReturn;
        }

        #region isValid()
        private bool isValid()
        {
            if (!bIsExistDate(dtpDate.Value) && (!bIsEdit))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Already exist record for this date.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            else if (!bIsValiedDateEntry(dtpDate.Value))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please enter previous record first.", MessageBoxButtons.OK, MessageBoxIcon.Information))
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
        #endregion

        private bool bIsExistDate(DateTime dDate)
        {
            bool bReturn = false;
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string commandText = " SELECT REPLICATIONCOUNTER FROM CUSTOMERFOOTFALL " +
                                 " WHERE [DATE] = '" + Convert.ToDateTime(dDate).ToString("dd-MMM-yyyy") + "'";
                                

            SqlCommand command = new SqlCommand(commandText, connection);

            string sResult = Convert.ToString(command.ExecuteScalar());
            connection.Close();

            if (string.IsNullOrEmpty(sResult))
                bReturn = true;
            
                return bReturn;
        }

        private void  CalTotalCustomer() 
        {
            int iTotCust = 0;
            int iExistCust = 0;
            int iNewCust = 0;
            int iHNICust = 0;

            if (string.IsNullOrEmpty(txtExistCust.Text.Trim()))
                iExistCust = 0;
            else
                iExistCust = Convert.ToInt32(txtExistCust.Text.Trim());

            if (string.IsNullOrEmpty(txtNewCust.Text.Trim()))
                iNewCust = 0;
            else
                iNewCust = Convert.ToInt32(txtNewCust.Text.Trim());

            if (string.IsNullOrEmpty(txtHNICust.Text.Trim()))
                iHNICust = 0;
            else
                iHNICust = Convert.ToInt32(txtHNICust.Text.Trim());

            iTotCust = iExistCust + iNewCust + iHNICust;


            lblTotalCust.Text= "Total : " +  iTotCust;

        }
         
        private void txtExistCust_TextChanged(object sender, EventArgs e)
        {
            CalTotalCustomer();
        }

        private void txtNewCust_TextChanged(object sender, EventArgs e)
        {
            CalTotalCustomer();
        }

        private void txtHNICust_TextChanged(object sender, EventArgs e)
        {
            CalTotalCustomer();
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
           // grdCustFootFall.ExportToXls("D:\\POS\\CustomerFootFall.xls");
            if (gridView.RowCount > 0)
            {
                using (var saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "Archivos Excel|*.xls";
                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        grdCustFootFall.ExportToXls(saveDialog.FileName);
                    }
                }
            }

        }
    }
}
