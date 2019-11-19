using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Pos.ApplicationService;
using Microsoft.Dynamics.Retail.Pos.Customer;
using Microsoft.Dynamics.Retail.Pos.Dialog;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch;
using Microsoft.Reporting.WinForms;
using Microsoft.Dynamics.Retail.Pos.BlankOperations;
using System.IO;
using DM = Microsoft.Dynamics.Retail.Pos.DataManager;

namespace BlankOperations
{
    public partial class frmRepairIssueReceive:frmTouchBase
    {
        public string CustAddress { get; set; }
        public string CustPhoneNo { get; set; }
        public IPosTransaction pos { get; set; }
        SqlConnection conn = new SqlConnection();
        DataTable dtRepairItem = new DataTable();
        DataTable dtItem = new DataTable();
        int iIssue = 0;
        string sEmail = "";
        string sCompanyName = string.Empty;
        Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations oBlank = new Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations();
        public string sPincode { get; set; }

        public DM.CustomerDataManager customerDataManager = new DM.CustomerDataManager(
                LSRetailPosis.Settings.ApplicationSettings.Database.LocalConnection,
                LSRetailPosis.Settings.ApplicationSettings.Database.DATAAREAID);

        [Import]
        private IApplication application;

        enum ReceiptTransactionType
        {
            RepairIssueReceive = 14
        }

        enum IssueReceiveType
        {
            None = 0,
            Issue = 1,
            Receive = 2
        }

        public frmRepairIssueReceive()
        {
            InitializeComponent();
        }

        public frmRepairIssueReceive(IPosTransaction posTransaction, IApplication Application, int iIssueOrRec)
        {
            InitializeComponent();
            pos = posTransaction;
            application = Application;
            txtIssueRecNo.Text = GetOrderNum();
            cmbRepairType.DataSource = Enum.GetValues(typeof(IssueReceiveType));
            iIssue = iIssueOrRec;

            cmbRepairType.SelectedIndex = iIssue;
        }


        private void btnCustomerSearch_Click(object sender, EventArgs e)
        {
            Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch.frmCustomerSearch obfrm =
                new Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch.frmCustomerSearch(this);

            obfrm.ShowDialog();
        }

        #region GetReceiptNo()
        public string GetOrderNum()
        {
            string OrderNum = string.Empty;
            OrderNum = GetNextCustomerOrderID();
            return OrderNum;
        }

        public string GetNextCustomerOrderID()
        {
            try
            {
                ReceiptTransactionType transType = ReceiptTransactionType.RepairIssueReceive;
                string storeId = LSRetailPosis.Settings.ApplicationSettings.Terminal.StoreId;
                string terminalId = LSRetailPosis.Settings.ApplicationSettings.Terminal.TerminalId;
                string staffId = pos.OperatorId;
                string mask;

                string funcProfileId = LSRetailPosis.Settings.FunctionalityProfiles.Functions.ProfileId;
                orderNumber((int)transType, funcProfileId, out mask);
                if(string.IsNullOrEmpty(mask))
                    return string.Empty;
                else
                {
                    string seedValue = GetSeedVal().ToString();
                    return ReceiptMaskFiller.FillMask(mask, seedValue, storeId, terminalId, staffId);
                }

            }
            catch(Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        private void orderNumber(int transType, string funcProfile, out string mask)
        {
            SqlConnection conn = new SqlConnection();
            if(application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            string Val = string.Empty;
            try
            {
                string queryString = " SELECT MASK FROM RETAILRECEIPTMASKS WHERE FUNCPROFILEID='" + funcProfile.Trim() + "' " +
                                     " AND RECEIPTTRANSTYPE=" + transType;
                using(SqlCommand command = new SqlCommand(queryString, conn))
                {
                    if(conn.State != ConnectionState.Open)
                        conn.Open();
                    Val = Convert.ToString(command.ExecuteScalar());
                    mask = Val;
                }
            }
            finally
            {
                if(conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        private string GetSeedVal()
        {
            string sFuncProfileId = LSRetailPosis.Settings.FunctionalityProfiles.Functions.ProfileId;
            int iTransType = (int)ReceiptTransactionType.RepairIssueReceive;

            SqlConnection conn = new SqlConnection();
            if(application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            string Val = string.Empty;
            try
            {
                string queryString = "DECLARE @VAL AS INT  SELECT @VAL = CHARINDEX('#',mask) FROM RETAILRECEIPTMASKS WHERE FUNCPROFILEID ='" + sFuncProfileId + "'  AND RECEIPTTRANSTYPE = " + iTransType + " " +
                                     " SELECT  MAX(CAST(ISNULL(SUBSTRING(RepairIssueRecNo,@VAL,LEN(RepairIssueRecNo)),0) AS INTEGER)) + 1 from RetailRepairIssueReceiveHeader";
                using(SqlCommand command = new SqlCommand(queryString, conn))
                {
                    if(conn.State != ConnectionState.Open)
                        conn.Open();

                    Val = Convert.ToString(command.ExecuteScalar());
                    if(string.IsNullOrEmpty(Val))
                        Val = "1";

                    return Val;
                }
            }
            finally
            {
                if(conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        #endregion

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(txtKarigar.Text))
            {
                DataTable dtRepair = new DataTable();
                DataRow drRepair = null;
                conn = application.Settings.Database.Connection;
                if(conn.State == ConnectionState.Closed)
                    conn.Open();
                string commandText = string.Empty;

                if(Convert.ToInt16(cmbRepairType.SelectedValue) == (int)IssueReceiveType.Issue)
                {
                    commandText = "select distinct d.RepairId from RetailRepairDetail d" +
                                   " left join RetailRepairHdr h on d.RepairId=h.RepairId " +
                                   " where d.IsDelivered=0  and h.RepairType=2 ";  // RepairType=2 (StoreRepair)// and d.ISSUERECEIVE=0
                }
                else if(Convert.ToInt16(cmbRepairType.SelectedValue) == (int)IssueReceiveType.Receive)
                {
                    commandText = "select distinct d.RepairId from RetailRepairDetail d" +
                                  " left join RetailRepairHdr h on d.RepairId=h.RepairId " +
                                  " where d.IsDelivered=0 and d.ISSUERECEIVE=" + (int)IssueReceiveType.Issue + ""+
                                  " and h.RepairType=2 and d.KARIGAR='" + txtKarigar.Text.Trim() + "'";  // RepairType=2 (StoreRepair)
                }


                SqlCommand command = new SqlCommand(commandText, conn);
                command.CommandTimeout = 0;
                SqlDataAdapter adapter = new SqlDataAdapter(commandText, conn);

                adapter.Fill(dtRepair);

                if(conn.State == ConnectionState.Open)
                    conn.Close();

                Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch oSearch =
                    new Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch(dtRepair, drRepair = null, "Repair Search");
                oSearch.ShowDialog();
                drRepair = oSearch.SelectedDataRow;
                if(drRepair != null)
                {
                    txtRepairId.Text = string.Empty;
                    txtRepairId.Text = Convert.ToString(drRepair["RepairId"]);
                }
            }
            else
            {
                MessageBox.Show("Please select a karigar.");
                txtKarigar.Focus();
            }
        }

        private void txtRepairId_TextChanged(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(txtRepairId.Text.Trim()))
            {
                LoadLine(txtRepairId.Text);
            }
        }

        private void LoadLine(string sRepairId)
        {
            SqlConnection connection = new SqlConnection();
            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            string sbQuery = string.Empty;

            if(Convert.ToInt16(cmbRepairType.SelectedValue) == (int)IssueReceiveType.Issue)
            {
                sbQuery = "select cast(LINENUM as int) LINENUM from RetailRepairDetail" +
                         " where RepairId='" + sRepairId + "' and IsDelivered=0 and ISSUERECEIVE=0";
            }
            else if(Convert.ToInt16(cmbRepairType.SelectedValue) == (int)IssueReceiveType.Receive)
            {
                sbQuery = "select cast(LINENUM as int) LINENUM from RetailRepairDetail" +
                         " where RepairId='" + sRepairId + "' and IsDelivered=0 and ISSUERECEIVE=" + (int)IssueReceiveType.Issue + "";
            }
            else
            {
                MessageBox.Show("Please select issue / receive type.");
                cmbRepairType.Focus();
            }

            DataTable dtLine = new DataTable();
            if(!string.IsNullOrEmpty(sbQuery))
            {
                if(connection.State == ConnectionState.Closed)
                    connection.Open();
                using(SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection))
                {
                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if(reader.HasRows)
                        {
                            dtLine.Load(reader);
                        }
                        reader.Close();
                        reader.Dispose();
                    }
                    cmd.Dispose();
                }

                if(connection.State == ConnectionState.Open)
                    connection.Close();

                if(dtLine.Rows.Count > 0)
                {
                    cmbLine.DataSource = dtLine;
                    cmbLine.DisplayMember = "LINENUM";
                    cmbLine.ValueMember = "LINENUM";
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtRepairId.Text = "";
            cmbLine.DataSource = null;
        }

        private void getSkuDetails(string sRepairId, int iLineNo)
        {
            int iNoSku = 0;
            int iNoOfSetOf = 0;
            decimal dTotQty = 0;

            conn = application.Settings.Database.Connection;
            if(conn.State == ConnectionState.Closed)
                conn.Open();

            string commandText = string.Empty;
            commandText = " select d.RepairId " +
                           " ,d.LINENUM " +
                           " ,d.ITEMID " +
                           " ,d.INVENTDIMID " +
                           " ,d.PCS " +
                           " ,d.QTY " +
                           " ,d.CONFIGID " +
                           " ,d.CODE " +
                           " ,d.SIZEID " +
                           " ,d.STYLE " +
                           " ,d.NETTWT " +
                           " ,d.DIAWT " +
                           " ,d.STNWT " +
                           " ,d.BatchId " +
                           " ,d.ARTICLECODE " +
                           " ,d.JOBTYPE " +
                           " ,h.REPAIRBAGNO " +
                         " from RetailRepairDetail d left join RetailRepairHdr"+
                         " h on d.RepairId =h.RepairId where d.RepairId='" + sRepairId + "'" +
                         " and d.LINENUM=" + iLineNo + "" +
                         " and d.IsDelivered=0";

            SqlCommand command = new SqlCommand(commandText, conn);
            command.CommandTimeout = 0;
            SqlDataAdapter adapter = new SqlDataAdapter(commandText, conn);
            adapter.Fill(dtRepairItem);

            if(conn.State == ConnectionState.Open)
                conn.Close();

            if(dtRepairItem != null)
            {
                string s = sRepairId;
                DataColumn[] columns = new DataColumn[2];
                columns[0] = dtRepairItem.Columns["RepairId"];
                columns[1] = dtRepairItem.Columns["LINENUM"];
                dtRepairItem.PrimaryKey = columns;

                dtItem = dtRepairItem;
                grItems.DataSource = dtRepairItem.DefaultView;

                //Start : added on 26/05/2014
                //foreach(DataRow dr in dtRepairItem.Rows)
                //{
                //    iNoSku = iNoSku + 1;
                //    iNoOfSetOf = iNoOfSetOf + Convert.ToInt32(dr[1]);
                //    dTotQty = dTotQty + Convert.ToDecimal(dr[2]);
                //}

                //lblTotNoOfSKU.Text = Convert.ToString(iNoSku);
                //lblTotSetOf.Text = Convert.ToString(iNoOfSetOf);
                //lblTotQty.Text = Convert.ToString(dTotQty);
                //End : added on 26/05/2014
            }
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(txtRepairId.Text) && Convert.ToInt16(cmbLine.SelectedValue) > 0)
            {
                getSkuDetails(txtRepairId.Text, Convert.ToInt16(cmbLine.SelectedValue));
            }
        }

        private void SaveIssueReceive()
        {
            int iHeader = 0;
            int iDetails = 0;

            SqlTransaction transaction = null;

            #region  HEADER
            string commandText = " INSERT INTO [RetailRepairIssueReceiveHeader]([IssueReceiveDate],[RepairIssueRecNo]," +
                                 " [RETAILSTAFFID],[RETAILSTOREID],[RETAILTERMINALID]," +
                                 " [DATAAREAID],[CUSTACCOUNT],[CUSTNAME],[CUSTADDRESS],[CUSTPHONE],[IssueReceive],[KARIGAR])" +
                                 " VALUES(@IssueReceiveDate,@RepairIssueRecNo,@RETAILSTAFFID,@RETAILSTOREID," +
                                 " @RETAILTERMINALID,@DATAAREAID,@CUSTACCOUNT,@CUSTNAME,@CUSTADDRESS,@CUSTPHONE,@IssueReceive,@KARIGAR)";
            SqlConnection connection = new SqlConnection();
            try
            {
                if(application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;


                if(connection.State == ConnectionState.Closed)
                    connection.Open();

                transaction = connection.BeginTransaction();

                SqlCommand command = new SqlCommand(commandText, connection, transaction);
                command.Parameters.Clear();
                command.Parameters.Add("@RepairIssueRecNo", SqlDbType.NVarChar).Value = txtIssueRecNo.Text.Trim();
                command.Parameters.Add("@IssueReceiveDate", SqlDbType.DateTime).Value = dtTransDate.Value;
                command.Parameters.Add("@RETAILSTOREID", SqlDbType.NVarChar, 20).Value = ApplicationSettings.Terminal.StoreId;
                command.Parameters.Add("@RETAILTERMINALID", SqlDbType.NVarChar, 20).Value = ApplicationSettings.Terminal.TerminalId;
                command.Parameters.Add("@RETAILSTAFFID", SqlDbType.NVarChar, 20).Value = pos.OperatorId;

                if(application != null)
                    command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = application.Settings.Database.DataAreaID;
                else
                    command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;

                command.Parameters.Add("@CUSTACCOUNT", SqlDbType.NVarChar, 20).Value = Convert.ToString(txtCustomerAccount.Text);
                command.Parameters.Add("@CUSTNAME", SqlDbType.NVarChar, 20).Value = Convert.ToString(txtCustomerName.Text);
                command.Parameters.Add("@CUSTADDRESS", SqlDbType.NVarChar, 20).Value = Convert.ToString(txtCustomerAddress.Text);
                command.Parameters.Add("@CUSTPHONE", SqlDbType.NVarChar, 20).Value = Convert.ToString(txtPhoneNumber.Text);
                command.Parameters.Add("@IssueReceive", SqlDbType.Int).Value = Convert.ToInt16(cmbRepairType.SelectedValue);
                command.Parameters.Add("@KARIGAR", SqlDbType.NVarChar, 20).Value = Convert.ToString(txtKarigar.Text);
                

                command.CommandTimeout = 0;
                iHeader = command.ExecuteNonQuery();

                if(iHeader == 1)
                {
                    if(dtItem != null && dtItem.Rows.Count > 0)
                    {
                        #region  DETAILS

                        string commandDetail = " INSERT INTO [RetailRepairIssueReceiveDetails]([RepairIssueRecNo],[RepairId],[LINENUM],[ITEMID]," +
                                                         " [INVENTDIMID],[PCS],[QTY],[CONFIGID],[CODE],[SIZEID]" +
                                                         " ,[STYLE],[NETTWT],[DIAWT],[STNWT],[BatchId],[ARTICLECODE])" +
                                                         " VALUES(@RepairIssueRecNo  ,@RepairId , @LINENUM,@ITEMID,@INVENTDIMID,@PCS," +
                                                         " @QTY,@CONFIGID,@CODE,@SIZEID,@STYLE,@NETTWT,@DIAWT,@STNWT,@BatchId,@ARTICLECODE) ";

                        commandDetail += " UPDATE RetailRepairDetail SET ISSUERECEIVE=" + Convert.ToInt16(cmbRepairType.SelectedValue) + ",KARIGAR=@KARIGAR" +
                                        " WHERE RepairId=@RepairId AND LINENUM=@LINENUM";

                        for(int ItemCount = 0; ItemCount < dtItem.Rows.Count; ItemCount++)
                        {

                            SqlCommand cmdDetail = new SqlCommand(commandDetail, connection, transaction);
                            cmdDetail.Parameters.Add("@RepairIssueRecNo", SqlDbType.NVarChar, 20).Value = txtIssueRecNo.Text.Trim();
                            cmdDetail.Parameters.Add("@RepairId", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtItem.Rows[ItemCount]["RepairId"]);
                            cmdDetail.Parameters.Add("@LINENUM", SqlDbType.Int).Value = Convert.ToInt16(dtItem.Rows[ItemCount]["LINENUM"]);

                            if(string.IsNullOrEmpty(Convert.ToString(dtItem.Rows[ItemCount]["ITEMID"])))
                                cmdDetail.Parameters.Add("@ITEMID", SqlDbType.NVarChar, 20).Value = DBNull.Value;
                            else
                                cmdDetail.Parameters.Add("@ITEMID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtItem.Rows[ItemCount]["ITEMID"]);

                            cmdDetail.Parameters.Add("@INVENTDIMID", SqlDbType.NVarChar, 20).Value = ApplicationSettings.Terminal.TerminalId;
                            cmdDetail.Parameters.Add("@PCS", SqlDbType.Decimal).Value = Convert.ToDecimal(dtItem.Rows[ItemCount]["PCS"]);

                            if(string.IsNullOrEmpty(Convert.ToString(dtItem.Rows[ItemCount]["QTY"])))
                                cmdDetail.Parameters.Add("@QTY", SqlDbType.Decimal).Value = DBNull.Value;
                            else
                                cmdDetail.Parameters.Add("@QTY", SqlDbType.Decimal).Value = Convert.ToDecimal(dtItem.Rows[ItemCount]["QTY"]);

                            cmdDetail.Parameters.Add("@CONFIGID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtItem.Rows[ItemCount]["CONFIGID"]);
                            cmdDetail.Parameters.Add("@CODE", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtItem.Rows[ItemCount]["CODE"]);
                            cmdDetail.Parameters.Add("@SIZEID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtItem.Rows[ItemCount]["SIZEID"]);
                            cmdDetail.Parameters.Add("@STYLE", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtItem.Rows[ItemCount]["STYLE"]);
                            cmdDetail.Parameters.Add("@NETTWT", SqlDbType.Decimal).Value = Convert.ToDecimal(dtItem.Rows[ItemCount]["NETTWT"]);
                            cmdDetail.Parameters.Add("@DIAWT", SqlDbType.Decimal).Value = Convert.ToDecimal(dtItem.Rows[ItemCount]["DIAWT"]);
                            cmdDetail.Parameters.Add("@STNWT", SqlDbType.Decimal).Value = Convert.ToDecimal(dtItem.Rows[ItemCount]["STNWT"]);
                            cmdDetail.Parameters.Add("@BatchId", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtItem.Rows[ItemCount]["BatchId"]);
                            cmdDetail.Parameters.Add("@ARTICLECODE", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtItem.Rows[ItemCount]["ARTICLECODE"]);
                            cmdDetail.Parameters.Add("@KARIGAR", SqlDbType.NVarChar, 20).Value = Convert.ToString(txtKarigar.Text);

                            cmdDetail.CommandTimeout = 0;
                            iDetails = cmdDetail.ExecuteNonQuery();
                            cmdDetail.Dispose();

                        }
                        #endregion
                    }
                }
                transaction.Commit();
                command.Dispose();
                transaction.Dispose();


                if(iHeader == 1 || iDetails > 0)
                {

                    string sCustName = txtCustomerName.Text.Trim();

                    using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Order has been created successfully.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);

                        try
                        {
                            PrintVoucher();
                        }
                        catch { }
                    }
                }
                else
                {
                    using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("DataBase error occured.Please try again later.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    }
                }
            }

            #endregion

            #region Exception
            catch(Exception ex)
            {
                transaction.Rollback();
                transaction.Dispose();

                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(ex.Message.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }
            }
            finally
            {
                if(connection.State == ConnectionState.Open)
                    connection.Close();
            }
            #endregion
        }


        public void PrintVoucher()
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            sCompanyName = oBlank.GetCompanyName(conn);//aded on 14/04/2014 R.Hossain
            string sIssRecTittle = "";
            if(cmbRepairType.SelectedIndex == 1)
                sIssRecTittle = "REPAIR TRANSFER TO CARRIGER";
            else
                sIssRecTittle = "REPAIR RECEIVE FROM CARRIGER";

            //datasources
            List<ReportDataSource> rds = new List<ReportDataSource>();
            rds.Add(new ReportDataSource("Header", (DataTable)GetHeaderInfo()));
            rds.Add(new ReportDataSource("Detail", (DataTable)GetDetailInfo()));

            //parameters
            List<ReportParameter> rps = new List<ReportParameter>();
            rps.Add(new ReportParameter("StoreName", string.IsNullOrEmpty(ApplicationSettings.Terminal.StoreName) ? " " : ApplicationSettings.Terminal.StoreName, true));
            rps.Add(new ReportParameter("StoreAddress", string.IsNullOrEmpty(ApplicationSettings.Terminal.StoreAddress) ? " " : ApplicationSettings.Terminal.StoreAddress, true));
            rps.Add(new ReportParameter("StorePhone", string.IsNullOrEmpty(ApplicationSettings.Terminal.StorePhone) ? " " : ApplicationSettings.Terminal.StorePhone, true));
            rps.Add(new ReportParameter("Title", sIssRecTittle, true));
            rps.Add(new ReportParameter("CompName", sCompanyName, true));
            rps.Add(new ReportParameter("pPincode", sPincode));

            string reportName = @"rptRepairIssueRecVoucher";
            string reportPath = @"Microsoft.Dynamics.Retail.Pos.BlankOperations.Report." + reportName + ".rdlc";



            RdlcViewer rptView = new RdlcViewer(sIssRecTittle, reportPath, rds, rps, null);
            rptView.ShowDialog();
        }

        public DataTable GetHeaderInfo()
        {
            DataTable dtHeader = new DataTable();
            dtHeader.Columns.Add("INVOICENO", typeof(string));
            dtHeader.Columns.Add("TRANSDATE", typeof(DateTime));
            dtHeader.Columns.Add("CUSTID", typeof(string));
            dtHeader.Columns.Add("CUSTNAME", typeof(string));
            dtHeader.Columns.Add("CUSTADD", typeof(string));
            dtHeader.Columns.Add("CUSTPHONE", typeof(string));
            dtHeader.Columns.Add("ISSUERECEIVE", typeof(string));
            dtHeader.Columns.Add("CustEmail", typeof(string));

            DataRow dr = dtHeader.NewRow();
            dr["INVOICENO"] = txtIssueRecNo.Text;
            dr["TRANSDATE"] = dtTransDate.Value;
            dr["CUSTID"] = txtCustomerAccount.Text;

            #region Customer Address format
            var SelectedCust = customerDataManager.GetTransactionalCustomer(txtCustomerAccount.Text);
            dr["CUSTNAME"] = txtCustomerName.Text;
            dr["CUSTADD"] = Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations.AddressLines(SelectedCust);
            sPincode = SelectedCust.PostalCode;
            sEmail = SelectedCust.Email;
            #endregion
            dr["CUSTPHONE"] = txtPhoneNumber.Text;
            dr["ISSUERECEIVE"] = cmbRepairType.Text;
            dr["CustEmail"] = sEmail;

            dtHeader.Rows.Add(dr);

            return dtHeader;
        }

        public DataTable GetDetailInfo()
        {
            string sSaleItem = string.Empty;
            DataTable dtDetails = new DataTable();
            dtDetails.Columns.Add("REPAIRID", typeof(string));
            dtDetails.Columns.Add("LINENUM", typeof(string));
            dtDetails.Columns.Add("ITEMID", typeof(string));
            dtDetails.Columns.Add("PCS", typeof(decimal));
            dtDetails.Columns.Add("QTY", typeof(decimal));
            dtDetails.Columns.Add("NETTWT", typeof(decimal));
            dtDetails.Columns.Add("CODE", typeof(string));
            dtDetails.Columns.Add("SIZEID", typeof(string));
            dtDetails.Columns.Add("STYLE", typeof(string));
            dtDetails.Columns.Add("DIAWT", typeof(decimal));
            dtDetails.Columns.Add("STNWT", typeof(decimal));
            dtDetails.Columns.Add("BatchId", typeof(string));

            int i = 1;
            foreach(DataRow item in dtItem.Rows)
            {
                DataRow dr = dtDetails.NewRow();
                dr["REPAIRID"] = item["REPAIRID"];
                dr["LINENUM"] = item["LINENUM"];
                dr["ITEMID"] = item["ITEMID"];
                dr["PCS"] = Convert.ToDecimal(item["PCS"]);
                dr["QTY"] = Convert.ToDecimal(item["QTY"]);
                dr["NETTWT"] = Convert.ToDecimal(item["NETTWT"]);
                dr["CODE"] = Convert.ToString(item["CODE"]);
                dr["SIZEID"] = Convert.ToString(item["SIZEID"]);
                dr["STYLE"] = Convert.ToString(item["STYLE"]);
                dr["DIAWT"] = Convert.ToDecimal(item["DIAWT"]);
                dr["STNWT"] = Convert.ToDecimal(item["STNWT"]);
                dr["BatchId"] = Convert.ToString(item["BatchId"]);

                i++;
                dtDetails.Rows.Add(dr);
            }
            return dtDetails;
        }


        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if(ValidateControls())
            {
                SaveIssueReceive();
                ClearControls();
                this.Close();
            }
        }

        private void ClearControls()
        {
            txtRepairId.Text = string.Empty;
            txtIssueRecNo.Text = string.Empty;
            txtCustomerAccount.Text = "";
            cmbLine.DataSource = null;
            cmbRepairType.SelectedIndex = 0;
            txtCustomerAddress.Text = string.Empty;
            txtCustomerName.Text = string.Empty;
            txtPhoneNumber.Text = string.Empty;
            grItems.DataSource = null;
            dtItem = new DataTable();
            dtRepairItem = new DataTable();
            txtIssueRecNo.Text = GetOrderNum();
        }

        bool ValidateControls()
        {
            bool rResult = true;

            if(string.IsNullOrEmpty(txtKarigar.Text))
            {
                ShowMessage("Select a karigar");
                btnKarigarSearch.Focus();
                rResult = false;
            }

            if(dtItem == null)
            {
                ShowMessage("Altleast one item should be select.");
                btnSearch.Focus();
                rResult = false;
            }
            else
            {
                rResult = true;
            }
            return rResult;
        }

        public string ShowMessage(string _msg)
        {
            using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(_msg, MessageBoxButtons.OK, MessageBoxIcon.Information))
            {
                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                return _msg;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if(grdView.RowCount > 0)
            {
                DeleteSelectedRows(grdView);
            }
        }

        private void DeleteSelectedRows(DevExpress.XtraGrid.Views.Grid.GridView view)
        {
            if(view == null || view.SelectedRowsCount == 0) return;

            DataRow[] rows = new DataRow[view.SelectedRowsCount];
            for(int i = 0; i < view.SelectedRowsCount; i++)
                rows[i] = view.GetDataRow(view.GetSelectedRows()[i]);
            view.BeginSort();
            try
            {
                foreach(DataRow rn in rows)
                {
                    foreach(DataRow row in dtItem.Rows)
                    {
                        if(Convert.ToString(row["RepairId"]) == Convert.ToString(rn[0]) && Convert.ToString(row["LINENUM"]) == Convert.ToString(rn[1]))
                        {
                            row.Delete();
                            break;
                        }
                    }
                }
                dtItem.AcceptChanges();
            }
            finally
            {
                view.EndSort();
            }
        }

        private bool isKarigar(string sCustId)
        {
            SqlConnection conn = new SqlConnection();
            if(application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();

            commandText.Append("select isnull(ISKARIGAR,0) FROM CUSTTABLE WHERE ACCOUNTNUM='" + sCustId + "'");

            if(conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            bool bVal = Convert.ToBoolean(command.ExecuteScalar());

            if(conn.State == ConnectionState.Open)
                conn.Close();

            return bVal;

        }

        private void btnKarigarSearch_Click(object sender, EventArgs e)
        {
            Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch.frmCustomerSearch obfrm =
                new Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch.frmCustomerSearch(this, 1);

            obfrm.ShowDialog();

            if(!isKarigar(obfrm.SelectedCustomerId))
            {
                txtKarigar.Text = "";
                MessageBox.Show("Selected person is not karigar.");
                btnKarigarSearch.Focus();
            }
        }

    }
}
