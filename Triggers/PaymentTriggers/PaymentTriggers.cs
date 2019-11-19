//Microsoft Dynamics AX for Retail POS Plug-ins 
//The following project is provided as SAMPLE code. Because this software is "as is," we may not provide support services for it.

using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Triggers;
using System.Data;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using System;
using System.Text;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.SaleItem;
using Microsoft.CSharp.RuntimeBinder;
using System.Collections.ObjectModel;
using System.Linq;


namespace Microsoft.Dynamics.Retail.Pos.PaymentTriggers
{
    [Export(typeof(IPaymentTrigger))]
    public class PaymentTriggers : IPaymentTrigger
    {
        //Start:Nim
        #region Nim
        #region enum MetalType
        enum MetalType
        {
            Other = 0,
            Gold = 1,
            Silver = 2,
            Platinum = 3,
            Alloy = 4,
            Diamond = 5,
            Pearl = 6,
            Stone = 7,
            Consumables = 8,
            Watch = 11,
            LooseDmd = 12,
            Palladium = 13,
            Jewellery = 14,
            Metal = 15,
            PackingMaterial = 16,
            Certificate = 17,
        }
        #endregion

        private enum IdentityProof
        {
            Aadhar = 0,
            PAN = 1,
            Driving_License = 2,
            Passport_No = 3,
            Voter_Id = 4,
            Emp_Id = 5,
        }
        decimal dGoldBookingRatePercentage = 0m; 
        decimal dSaleQty = 0m;
        

        enum CRWRetailDiscPermission // added on 04/05/2017
        {
            Cashier = 0,
            Salesperson = 1,
            Manager = 2,
            Other = 3,
            Manager2 = 4,
        }

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
        int iIsConvertToAdv = 0;



        /// <summary>
        /// retail =1 in inventtable , item can only sale
        /// Dev on  : 17/04/2014 by : RHossain
        /// </summary>
        /// <param name="sItemId"></param>
        /// <returns></returns>
        private bool IsRetailItem(string sItemId)
        {
            bool bRetailItem = false;

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "select RETAIL from inventtable WHERE ITEMID = '" + sItemId + "'";


            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            bRetailItem = Convert.ToBoolean(command.ExecuteScalar());
            if (connection.State == ConnectionState.Open)
                connection.Close();

            return bRetailItem;
        }

        private bool getCostPrice(string sItemId, decimal dSalesValue)
        {
            bool bReturn = false;

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "select ABS(CAST(ISNULL(CostPrice + TRANSFERCOSTPRICE,0)AS DECIMAL(28,2))) from SKUTABLE_POSTED WHERE skunumber = '" + sItemId + "'";


            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            decimal _dSalesValue = Convert.ToDecimal(command.ExecuteScalar());

            if (_dSalesValue <= dSalesValue)
                bReturn = true;
            else
                bReturn = false;

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return bReturn;
        }

        private bool isItemIsAvailable(string sItemId)
        {
            bool bReturn = false;

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "select isnull(SKUNUMBER,'') from SKUTABLETRANS WHERE skunumber = '" + sItemId + "' AND ISAVAILABLE=1";


            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            string _Sku = Convert.ToString(command.ExecuteScalar());

            if (!string.IsNullOrEmpty(_Sku))
                bReturn = true;
            else
                bReturn = false;

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return bReturn;
        }

        private bool isPANExistForCustomer(string sCustId)
        {
            bool bReturn = false;
            int iValue1 = 0;
            int iValue2 = 0;
            string sValue1 = "";
            string sValue2 = "";


            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "SELECT GOVTIDENTITY,GOVTIDENTITY2,isnull(GOVTIDNO,''),isnull(GOVTIDNO2,'') FROM CUSTTABLE where ACCOUNTNUM= '" + sCustId + "'";

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    iValue1 = Convert.ToInt16(reader.GetValue(0));
                    iValue2 = Convert.ToInt16(reader.GetValue(1));
                    sValue1 = Convert.ToString(reader.GetValue(2));
                    sValue2 = Convert.ToString(reader.GetValue(3));
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();


            if ((iValue1 == (int)IdentityProof.PAN && sValue1 != "") || (iValue2 == (int)IdentityProof.PAN && sValue2 != ""))
                bReturn = true;
            else
                bReturn = false;

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return bReturn;
        }

        private bool isItemIsAlreadyReturn(string sReturnTransId, string sStore, string sTerminal, int iLine)
        {
            bool bReturn = false;

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "select TRANSACTIONID from RETAILTRANSACTIONSALESTRANS where RETURNTRANSACTIONID='" + sReturnTransId + "' " +
                                " and RETURNLINENUM=" + iLine + " and RETURNSTORE='" + sStore + "' " +
                                " and RETURNTERMINALID='" + sTerminal + "'  and RECEIPTID!='' and TRANSACTIONSTATUS=0"; //added on 260418 TRANSACTIONSTATUS


            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            string _Sku = Convert.ToString(command.ExecuteScalar());

            if (!string.IsNullOrEmpty(_Sku))
                bReturn = true;
            else
                bReturn = false;

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return bReturn;
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

        private bool isRetailItem(string itemid)
        {
            SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append(" DECLARE @ISVALID INT");
            sbQuery.Append(" IF EXISTS (SELECT ITEMID FROM INVENTTABLE WHERE ITEMID = '" + itemid + "'");
            sbQuery.Append(" AND DATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "' AND RETAIL = 1) ");
            sbQuery.Append(" AND EXISTS (SELECT SkuNumber FROM SKUTableTrans WHERE SkuNumber = '" + itemid + "'");
            sbQuery.Append(" AND DATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "')");
            sbQuery.Append(" BEGIN SET @ISVALID = 1 END ELSE BEGIN SET @ISVALID = 0 END SELECT ISNULL(@ISVALID,0)");

            if (SqlCon.State == ConnectionState.Closed)
                SqlCon.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), SqlCon);
            return Convert.ToBoolean(cmd.ExecuteScalar());

        }

        private bool IsCertificateItem(string sItemId)
        {
            bool bCertificateItem = false;

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "SELECT SKUCERTIFICATE FROM SKUTABLE_POSTED  WHERE SkuNumber = '" + sItemId + "'";


            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            bCertificateItem = Convert.ToBoolean(command.ExecuteScalar());
            if (connection.State == ConnectionState.Open)
                connection.Close();

            return bCertificateItem;
        }

        private int getMetalType(string sItemId)
        {
            int iMetalType = 100;
            SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT METALTYPE FROM INVENTTABLE WHERE ITEMID='" + sItemId + "'");

            if (SqlCon.State == ConnectionState.Closed)
                SqlCon.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), SqlCon);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    iMetalType = (int)reader.GetValue(0);
                }
            }
            if (SqlCon.State == ConnectionState.Open)
                SqlCon.Close();
            return iMetalType;
        }

        private bool chkOGTaxApplicable()
        {
            bool bResult = false;
            SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT isnull(OGTaxApplicable,0) FROM RETAILPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

            if (SqlCon.State == ConnectionState.Closed)
                SqlCon.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), SqlCon);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    int iResult = (int)reader.GetValue(0);

                    if (iResult == 1)
                        bResult = true;
                    else
                        bResult = false;

                }
            }
            if (SqlCon.State == ConnectionState.Open)
                SqlCon.Close();
            return bResult;
        }

        private string AdjustmentItemID()
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT TOP(1) ADJUSTMENTITEMID FROM [RETAILPARAMETERS] ");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToString(cmd.ExecuteScalar());
        }

        private decimal GetGoldBookingRatePer()
        {
            decimal dFixRatePct = 0M;
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append(" DECLARE @FIXEDRATEVAL AS NUMERIC(32,2)");
            sbQuery.Append(" DECLARE @ORDERQTY AS NUMERIC(32,2)");
            sbQuery.Append("SELECT ISNULL(FIXEDRATEADVANCEPCT,0) FROM RETAILPARAMETERS WHERE DATAAREAID = '" + application.Settings.Database.DataAreaID + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            dFixRatePct = Convert.ToDecimal(cmd.ExecuteScalar());
            if (connection.State == ConnectionState.Open)
                connection.Close();
            return dFixRatePct;
        }

        private decimal GetIncomeExpMaxCashLimit()
        {
            decimal dResult = 0M;
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT ISNULL(IncomeExpMaxCashLimit,0) FROM RETAILPARAMETERS WHERE DATAAREAID = '" + application.Settings.Database.DataAreaID + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            dResult = Convert.ToDecimal(cmd.ExecuteScalar());
            if (connection.State == ConnectionState.Open)
                connection.Close();

            return dResult;
        }

        private decimal GetRoundOffMOPLimit() 
        {
            decimal dResult = 0M;
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT CAST(ISNULL(RoundOffLimit,0) AS DECIMAL(28,2)) AS RoundOffLimit FROM RETAILPARAMETERS WHERE DATAAREAID = '" + application.Settings.Database.DataAreaID + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            dResult = Convert.ToDecimal(cmd.ExecuteScalar());
            if (connection.State == ConnectionState.Open)
                connection.Close();

            return dResult;
        }

        private decimal GetGSSMaxCashLimit()
        {
            decimal dResult = 0M;
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT ISNULL(GSSMaxCashLimit,0) FROM RETAILPARAMETERS WHERE DATAAREAID = '" + application.Settings.Database.DataAreaID + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            dResult = Convert.ToDecimal(cmd.ExecuteScalar());
            if (connection.State == ConnectionState.Open)
                connection.Close();
            return dResult;
        }

        private decimal GetOGPurchaseMaxCashLimit()
        {
            decimal dResult = 0M;
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();


            sbQuery.Append("SELECT ISNULL(OGPurchaseMaxCashLimit,0) FROM RETAILPARAMETERS WHERE DATAAREAID = '" + application.Settings.Database.DataAreaID + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            dResult = Convert.ToDecimal(cmd.ExecuteScalar());
            if (connection.State == ConnectionState.Open)
                connection.Close();

            return dResult;
        }

        private decimal GetGVMaxCashLimit()
        {
            decimal dResult = 0M;
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT ISNULL(GVMaxCashLimit,0) FROM RETAILPARAMETERS WHERE DATAAREAID = '" + application.Settings.Database.DataAreaID + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            dResult = Convert.ToDecimal(cmd.ExecuteScalar());
            if (connection.State == ConnectionState.Open)
                connection.Close();
            return dResult;
        }

        private decimal GetTodaysTotIncomeWithCash()
        {
            decimal dResult = 0M;
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            sbQuery.Append(" declare @Amt numeric(20,2) select CAST(SUM(ISNULL(abs(IE.amount),0)) AS DECIMAL(28,2)) AS AMOUNT");
            sbQuery.Append(" from RETAILTRANSACTIONINCOMEEXPENSETRANS IE");
            sbQuery.Append(" left join RETAILTRANSACTIONPAYMENTTRANS PT on IE.TRANSACTIONID=PT.TRANSACTIONID ");
            sbQuery.Append(" and IE.STORE=PT.STORE and IE.TERMINAL=PT.TERMINAL ");
            sbQuery.Append(" where IE.transdate=CONVERT(date, getdate()) and PT.TENDERTYPE=1 AND IE.ACCOUNTTYPE=0 and PT.TRANSACTIONSTATUS=0");
            sbQuery.Append(" group by ACCOUNTTYPE ,IE.TRANSDATE; select isnull(@Amt,0) Amount");

            //if (connection.State == ConnectionState.Open)
            //    connection.Close();

            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            dResult = Convert.ToDecimal(cmd.ExecuteScalar());

            return dResult;
        }

        private decimal GetTodaysTotExpenseWithCash()
        {
            decimal dResult = 0M;
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("select CAST(SUM(ISNULL(abs(IE.amount),0)) AS DECIMAL(28,2)) AS AMOUNT");
            sbQuery.Append(" from RETAILTRANSACTIONINCOMEEXPENSETRANS IE");
            sbQuery.Append(" left join RETAILTRANSACTIONPAYMENTTRANS PT on IE.TRANSACTIONID=PT.TRANSACTIONID ");
            sbQuery.Append(" and IE.STORE=PT.STORE and IE.TERMINAL=PT.TERMINAL ");
            sbQuery.Append(" where IE.transdate=CONVERT(date, getdate()) and PT.TENDERTYPE=1 AND IE.ACCOUNTTYPE=1 and PT.TRANSACTIONSTATUS=0");
            sbQuery.Append(" group by ACCOUNTTYPE ,IE.TRANSDATE");

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            dResult = Convert.ToDecimal(cmd.ExecuteScalar());

            //if (connection.State == ConnectionState.Closed)
            //    connection.Open();

            return dResult;
        }

        private decimal GetTodaysTotCashPaymentForSelectedCustomer(RetailTransaction retailTrans, LSRetailPosis.Transaction.CustomerPaymentTransaction custTrans)
        {
            decimal dTransCash = 0m;
            string sTableName = "DAILYCASHPAY" + ApplicationSettings.Terminal.TerminalId;

            DataTable dtCash = new DataTable();
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            if (conn.State == ConnectionState.Open)
                conn.Close();
            string sStoreNo = ApplicationSettings.Terminal.StoreId;
            string sQuery = "";
            DateTime dtTransDate = Convert.ToDateTime((DateTime.Now).ToShortDateString());

            sQuery = "GETDAILYCASHPAYFORACUSTOMER";

            SqlCommand command = new SqlCommand(sQuery, conn);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();

            command.Parameters.Add("@TransDate", SqlDbType.DateTime).Value = dtTransDate;
            if (retailTrans != null)
            {
                command.Parameters.Add("@CUSTACCOUNT", SqlDbType.NVarChar).Value = retailTrans.Customer.CustomerId;
                command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar).Value = retailTrans.TerminalId;
            }
            else
            {
                command.Parameters.Add("@CUSTACCOUNT", SqlDbType.NVarChar).Value = custTrans.Customer.CustomerId;
                command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar).Value = custTrans.TerminalId;
            }

            command.Parameters.Add("@STORENUMBER", SqlDbType.NVarChar).Value = sStoreNo;

            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daTrans = new SqlDataAdapter(command);
            daTrans.Fill(dtCash);

            if (dtCash != null && dtCash.Rows.Count > 0)
            {
                if (Convert.ToString(dtCash.Rows[0]["TOTAMOUNT"]) == string.Empty)
                    dTransCash = 0;
                else
                    dTransCash = Convert.ToDecimal(dtCash.Rows[0]["TOTAMOUNT"]);
            }

            return dTransCash;
        }

        private decimal GetTodaysTotCashPaymentForSelectedCustomerNegetive(RetailTransaction retailTrans, LSRetailPosis.Transaction.CustomerPaymentTransaction custTrans)
        {
            decimal dTransCash = 0m;
            string sTableName = "DAILYCASHPAY" + ApplicationSettings.Terminal.TerminalId;

            DataTable dtCash = new DataTable();
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            if (conn.State == ConnectionState.Open)
                conn.Close();
            string sStoreNo = ApplicationSettings.Terminal.StoreId;
            string sQuery = "";
            DateTime dtTransDate = Convert.ToDateTime((DateTime.Now).ToShortDateString());

            sQuery = "GETDAILYNEGETIVECASHPAYFORACUSTOMER";

            SqlCommand command = new SqlCommand(sQuery, conn);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();

            command.Parameters.Add("@TransDate", SqlDbType.DateTime).Value = dtTransDate;
            if (retailTrans != null)
            {
                command.Parameters.Add("@CUSTACCOUNT", SqlDbType.NVarChar).Value = retailTrans.Customer.CustomerId;
                command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar).Value = retailTrans.TerminalId;
            }
            else
            {
                command.Parameters.Add("@CUSTACCOUNT", SqlDbType.NVarChar).Value = custTrans.Customer.CustomerId;
                command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar).Value = custTrans.TerminalId;
            }

            command.Parameters.Add("@STORENUMBER", SqlDbType.NVarChar).Value = sStoreNo;

            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daTrans = new SqlDataAdapter(command);
            daTrans.Fill(dtCash);

            if (dtCash != null && dtCash.Rows.Count > 0)
            {
                if (Convert.ToString(dtCash.Rows[0]["TOTAMOUNT"]) == string.Empty)
                    dTransCash = 0;
                else
                    dTransCash = Convert.ToDecimal(dtCash.Rows[0]["TOTAMOUNT"]);
            }

            return dTransCash;
        }

        private void tableCreate(string sTableName)
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();

            commandText.Append(" SET TRANSACTION ISOLATION LEVEL SNAPSHOT ;IF NOT EXISTS (SELECT A.NAME FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTableName + "')");
            commandText.Append(" BEGIN CREATE TABLE " + sTableName + "(");
            commandText.Append(" TERMINALID NVARCHAR (20),NEGAMT NUMERIC (20,2),SRCASH int,IsTransAmt INT NOT NULL DEFAULT 0) END");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            command.ExecuteNonQuery();

            if (conn.State == ConnectionState.Open)
                conn.Close();

        }

        private void UpdateDailyCashIfo(string sTableName, string sTerminalId, decimal dEmpAmt)
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();


            commandText.Append("IF EXISTS (SELECT A.NAME FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTableName + "')");
            commandText.Append(" BEGIN IF NOT EXISTS (SELECT * FROM " + sTableName + " WHERE IsTransAmt = 1)");
            commandText.Append(" BEGIN  INSERT INTO  " + sTableName + "  VALUES('" + sTerminalId + "',");
            commandText.Append(" '" + dEmpAmt + "',0,1) END END");


            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            command.ExecuteNonQuery();
            if (conn.State == ConnectionState.Open)
                conn.Close();

        }

        private bool isSRToAdvance(int iTenderTypeId)
        {
            SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            if (SqlCon.State == ConnectionState.Closed)
                SqlCon.Open();

            string commandText = string.Empty;
            commandText = "DECLARE @CHANNEL bigint  SELECT @CHANNEL = ISNULL(RECID,0) FROM RETAILSTORETABLE WHERE STORENUMBER = '" + ApplicationSettings.Database.StoreID + "'" +
                          "SELECT SRTOADVANCE FROM RetailStoreTenderTypeTable WHERE TENDERTYPEID=" + iTenderTypeId + " AND CHANNEL = @CHANNEL ";

            SqlCommand command = new SqlCommand(commandText.ToString(), SqlCon);
            command.CommandTimeout = 0;
            bool sResult = Convert.ToBoolean(command.ExecuteScalar());
            return sResult;
        }

        private bool isRoundOff(int iTenderTypeId) 
        {
            SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            if (SqlCon.State == ConnectionState.Closed)
                SqlCon.Open();

            string commandText = string.Empty;
            commandText = "DECLARE @CHANNEL bigint  SELECT @CHANNEL = ISNULL(RECID,0) FROM RETAILSTORETABLE WHERE STORENUMBER = '" + ApplicationSettings.Database.StoreID + "'" +
                          "SELECT RoundOff FROM RetailStoreTenderTypeTable WHERE TENDERTYPEID=" + iTenderTypeId + " AND CHANNEL = @CHANNEL ";

            SqlCommand command = new SqlCommand(commandText.ToString(), SqlCon);
            command.CommandTimeout = 0;
            bool sResult = Convert.ToBoolean(command.ExecuteScalar());
            return sResult;
        }


        private bool isLoyaltyPayment(int iTenderTypeId)
        {
            SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            if (SqlCon.State == ConnectionState.Closed)
                SqlCon.Open();

            string commandText = string.Empty;
            commandText = "DECLARE @CHANNEL bigint  SELECT @CHANNEL = ISNULL(RECID,0) FROM RETAILSTORETABLE WHERE STORENUMBER = '" + ApplicationSettings.Database.StoreID + "'" +
                          "SELECT LOYALTYPAYMENT FROM RetailStoreTenderTypeTable WHERE TENDERTYPEID=" + iTenderTypeId + " AND CHANNEL = @CHANNEL ";

            SqlCommand command = new SqlCommand(commandText.ToString(), SqlCon);
            command.CommandTimeout = 0;
            bool sResult = Convert.ToBoolean(command.ExecuteScalar());
            return sResult;
        }

        private decimal getNetAmount(string sReceiptId)
        {
            SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            if (SqlCon.State == ConnectionState.Closed)
                SqlCon.Open();

            string commandText = string.Empty;
            commandText = " select abs(CAST(ISNULL(NETAMOUNT,0) AS DECIMAL(20,2))) from RETAILTRANSACTIONTABLE where receiptid='" + sReceiptId + "'";

            SqlCommand command = new SqlCommand(commandText.ToString(), SqlCon);
            command.CommandTimeout = 0;
            decimal sResult = Convert.ToDecimal(command.ExecuteScalar());
            return sResult;
        }

        private void TempTableCreate(string sTableName)
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();


            commandText.Append("SET TRANSACTION ISOLATION LEVEL SNAPSHOT ;IF NOT EXISTS (SELECT A.NAME FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTableName + "')");
            commandText.Append(" BEGIN CREATE TABLE " + sTableName + "(");
            commandText.Append(" TERMINALID NVARCHAR (20),NEGAMT NUMERIC (20,2),SRCASH int,IsTransAmt INT NOT NULL DEFAULT 0) END");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            command.ExecuteNonQuery();

            if (conn.State == ConnectionState.Open)
                conn.Close();

        }

        private void UpdateNegCashIfo(string sTableName, string sTerminalId, decimal dEmpAmt, int iSRCashPay)
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();


            commandText.Append("IF EXISTS (SELECT A.NAME FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTableName + "')");
            commandText.Append(" BEGIN  INSERT INTO  " + sTableName + "  VALUES('" + sTerminalId + "',");
            commandText.Append(" '" + dEmpAmt + "'," + iSRCashPay + ",0) END");


            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            command.ExecuteNonQuery();
            if (conn.State == ConnectionState.Open)
                conn.Close();

        }

        private void UpdateTransCashInfo(string sTableName, string sTerminalId,string sCustAcc)
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();

            commandText.Append(" DECLARE @EXISTAMT numeric(20,2) BEGIN ");
            commandText.Append(" SELECT @EXISTAMT = CAST(ISNULL(sum(PT.AMOUNTCUR),0)AS DECIMAL(28,2))");
            commandText.Append(" FROM RETAILTRANSACTIONSALESTRANS A ");
            commandText.Append(" INNER JOIN RETAIL_CUSTOMCALCULATIONS_TABLE B ON A.TRANSACTIONID = B.TRANSACTIONID ");
            commandText.Append(" AND A.LINENUM = B.LINENUM ");
            commandText.Append(" AND A.TERMINALID = B.TERMINALID ");
            commandText.Append(" join RETAILTRANSACTIONPAYMENTTRANS PT on A.TRANSACTIONID=PT.TRANSACTIONID");
            commandText.Append(" WHERE CUSTACCOUNT='" + sCustAcc + "'");
            commandText.Append(" AND A.TRANSDATE ='" + (DateTime.Now).ToString("dd-MMM-yyyy") + "'");//
            commandText.Append(" AND A.TransactionStatus = 0 ");
            commandText.Append(" and PT.TENDERTYPE=1");//--(Cash)
            commandText.Append(" and pt.TRANSACTIONSTATUS=0");
            commandText.Append(" and B.TRANSACTIONTYPE in(1,3)");//og purchase and exchange

            commandText.Append("IF EXISTS (SELECT A.NAME FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTableName + "')");
            commandText.Append(" BEGIN IF NOT EXISTS (SELECT * FROM " + sTableName + "  WHERE isTransAmt=2)");
            commandText.Append(" INSERT INTO  " + sTableName + "  VALUES('" + sTerminalId + "',");
            commandText.Append(" @EXISTAMT,0,2)END END");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            command.ExecuteNonQuery();
            if (conn.State == ConnectionState.Open)
                conn.Close();

        }

        private DataTable GetDataTableInfo(string sStringSql)
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            try
            {
                SqlCommand command = new SqlCommand(sStringSql.ToString(), conn);
                DataTable CustAdvDt = new DataTable();

                SqlDataAdapter SqlDa = new SqlDataAdapter(command);
                SqlDa.Fill(CustAdvDt);
                return CustAdvDt;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private int getUserDiscountPermissionId()
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("select RETAILDISCPERMISSION from RETAILSTAFFTABLE where STAFFID='" + ApplicationSettings.Terminal.TerminalOperator.OperatorId + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);

            int iResult = 0;
            iResult = Convert.ToInt16(cmd.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return iResult;
        }

        private DataTable GetAdvDetailInfo(string sStringSql)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = sStringSql;

                DataTable CustAdvDt = new DataTable();

                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(CustAdvDt);
                return CustAdvDt;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private int isSuvarnaVrudhi(string sOrderNo)
        {
            SqlConnection connection = new SqlConnection();
            connection = ApplicationSettings.Database.LocalConnection;
            int iResult = 0;

            string commandText = string.Empty;
            commandText = "select isnull(SUVARNAVRUDHI,0) SUVARNAVRUDHI from CUSTORDER_HEADER  where ORDERNUM='" + sOrderNo + "'";
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            iResult = Convert.ToInt16(command.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();
            return iResult;
        }

        private void getOrderAdvanceAmount(string sOrderNo, ref decimal dTotAdvAmt)
        {
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string commandText = " SELECT SUM(ISNULL(ADVANCEAMT,0)) FROM CUSTORDER_DETAILS WHERE ORDERNUM = '" + sOrderNo + "' ";


            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    dTotAdvAmt = Convert.ToDecimal(reader.GetValue(0));
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

        private decimal GetGoldBookingRatePer(ref decimal dAdvTaxPerc, ref decimal dSVT)
        {
            decimal dFixRatePct = 0M;
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string commandText = " SELECT ISNULL(FIXEDRATEADVANCEPCT,0),ISNULL(ADVANCETAXPERCENTAGE,0),ISNULL(SVADVTOLERANCE,0) FROM RETAILPARAMETERS WHERE DATAAREAID = '" + application.Settings.Database.DataAreaID + "'";

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    dFixRatePct = Convert.ToDecimal(reader.GetValue(0));
                    dAdvTaxPerc = Convert.ToDecimal(reader.GetValue(1));
                    dSVT = Convert.ToDecimal(reader.GetValue(2));
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return dFixRatePct;
        }

        private string OrderNum(out bool isRepair)
        {
            isRepair = false;
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string commandText = " SELECT CUSTORDER,CUSTID FROM RETAILTEMPTABLE WHERE ID=1 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE
            SqlCommand command = new SqlCommand(commandText, connection);
            SqlDataReader reader = command.ExecuteReader();
            string strCustOrder = string.Empty;
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    strCustOrder = Convert.ToString(reader.GetValue(reader.GetOrdinal("CUSTORDER")));
                    string sIsRepair = Convert.ToString(reader.GetValue(reader.GetOrdinal("CUSTID")));
                    if (sIsRepair == "REPAIR")
                    {
                        isRepair = true;
                    }
                }
            }
            connection.Close();
            return strCustOrder;

        }

        private decimal GetPANMandatAmount() // added on 23/05/2014 for give PAN  if invoiceamount > 20000000(example) then PAN enter.
        {
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string commandText = "SELECT ISNULL(PANMANDATORYTRANSLIMIT,0) FROM RETAILPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'";

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;

            decimal dAmount = Convert.ToDecimal(command.ExecuteScalar());
            return dAmount;
        }

        private string getBussinesAgentCode()
        {
            string sSCType = string.Empty;
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "select h.PERSONNELNUMBER Code,d.NAME as Name from dbo.HCMWORKER h" +
                                " left join dbo.DIRPARTYTABLE as d on d.RECID = h.PERSON ";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand cmd = new SqlCommand(commandText, connection);
            SqlDataAdapter daSC = new SqlDataAdapter(cmd);
            DataTable dtSC = new DataTable();
            daSC.Fill(dtSC);
            if (dtSC != null && dtSC.Rows.Count > 0)
            {
                Dialog.WinFormsTouch.frmGenericSearch oSearch = new Dialog.WinFormsTouch.frmGenericSearch(dtSC, null, "Marketing Associate List");
                this.Application.ApplicationFramework.POSShowForm(oSearch);
                DataRow dr = null;
                dr = oSearch.SelectedDataRow;
                if (dr != null)
                {
                    sSCType = Convert.ToString(dr["Code"]);
                }
            }
            else
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No record found.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }
            }
            return sSCType;
        }

        private string GetSalesAgent()
        {
            string sSCType = string.Empty;
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = string.Empty;

            commandText = "select SALESAGENTCODE Code,Name from CRWEXTERNALSALESAGENT";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand cmd = new SqlCommand(commandText, connection);
            SqlDataAdapter daSC = new SqlDataAdapter(cmd);
            DataTable dtSC = new DataTable();
            daSC.Fill(dtSC);
            if (dtSC != null && dtSC.Rows.Count > 0)
            {
                Dialog.WinFormsTouch.frmGenericSearch oSearch = new Dialog.WinFormsTouch.frmGenericSearch(dtSC, null, "Sales Associate List");
                this.Application.ApplicationFramework.POSShowForm(oSearch);
                DataRow dr = null;
                dr = oSearch.SelectedDataRow;
                if (dr != null)
                {
                    sSCType = Convert.ToString(dr["Code"]);
                }
            }
            else
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No record found.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }
            }
            return sSCType;
        }

        #endregion

        //End:Nim

        #region Constructor - Destructor

        public PaymentTriggers()
        {

            // Get all text through the Translation function in the ApplicationLocalizer
            // TextID's for PaymentTriggers are reserved at 50400 - 50449

        }

        #endregion

        #region IPaymentTriggers Members

        public void PrePayCustomerAccount(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction, decimal amount)
        {
            LSRetailPosis.ApplicationLog.Log("PaymentTriggers.PrePayCustomerAccount", "Before charging to a customer account", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PrePayCardAuthorization(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction, ICardInfo cardInfo, decimal amount)
        {
            LSRetailPosis.ApplicationLog.Log("PaymentTriggers.PrePayCardAuthorization", "Before the EFT authorization", LSRetailPosis.LogTraceLevel.Trace);
        }

        /// <summary>
        /// <example><code>
        /// // In order to delete the already-added payment you use the following code:
        /// if (retailTransaction.TenderLines.Count > 0)
        /// {
        ///     retailTransaction.TenderLines.RemoveLast();
        ///     retailTransaction.LastRunOpererationIsValidPayment = false;
        /// }
        /// </code></example>
        /// </summary>
        public void OnPayment(IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("PaymentTriggers.OnPayment", "On the addition of a tender...", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PrePayment(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction, object posOperation, string tenderId)
        {
            //Start:Nim
            int isIssueToGC = 0;
            bool bIsOGPurchase = false;
            decimal dTransDueAmt = 0m;
            string sCustId = "";

            RetailTransaction retailTransaction = posTransaction as RetailTransaction;
            LSRetailPosis.Transaction.CustomerPaymentTransaction custTrans = posTransaction as LSRetailPosis.Transaction.CustomerPaymentTransaction;

           
            #region PAN validate
            if (retailTransaction != null)
            {
                if (retailTransaction.TransSalePmtDiff > 0)
                    dTransDueAmt = retailTransaction.TransSalePmtDiff;


                #region GMA discount buttan click is mandatory
                decimal dGMAAdvAmt = Convert.ToDecimal(retailTransaction.PartnerData.GMATotAdvance);

                if (dGMAAdvAmt > 0 && retailTransaction.PartnerData.APPLYGMADISC == false)
                {
                    MessageBox.Show("GMA discount is not applied");
                    preTriggerResult.ContinueOperation = false;
                    return;
                }
                #endregion

                sCustId = retailTransaction.Customer.CustomerId;

                if (retailTransaction.IncomeExpenseAmounts == 0
                    && string.IsNullOrEmpty(Convert.ToString(retailTransaction.PartnerData.SalesAssociateCode))
                    && string.IsNullOrEmpty(Convert.ToString(retailTransaction.PartnerData.BussinesAgentCode))
                    )
                {
                    string sSalesAssociateCode = "";
                    sSalesAssociateCode = GetSalesAgent();
                    retailTransaction.PartnerData.SalesAssociateCode = sSalesAssociateCode;

                    string sBussinessAgentCode = "";
                    sBussinessAgentCode = getBussinesAgentCode();
                    retailTransaction.PartnerData.BussinesAgentCode = sBussinessAgentCode;
                }
            }

            if (custTrans != null && dTransDueAmt == 0)
            {
                if (custTrans.TransSalePmtDiff > 0)
                    dTransDueAmt = custTrans.TransSalePmtDiff;

                sCustId = custTrans.Customer.CustomerId;
            }
            decimal dPANMandatAmt = 0m;
            dPANMandatAmt = GetPANMandatAmount();

            if (dTransDueAmt > 0 && dTransDueAmt >= dPANMandatAmt)
            {
                if (!isPANExistForCustomer(sCustId))
                {
                    MessageBox.Show("Please update PAN for the selected customer account : " + sCustId + "");
                    preTriggerResult.ContinueOperation = false;
                    return;
                }
            }
            #endregion




            if (retailTransaction != null
                 && retailTransaction.IncomeExpenseAmounts == 0
                 && retailTransaction.PartnerData.LOYALTYASKINGDONE == false
                   && retailTransaction.SaleItems.Count > 0)
            {
                foreach (SaleLineItem SLineItem in retailTransaction.SaleItems)
                {
                    if (!retailTransaction.SaleIsReturnSale)
                    {
                        if (((LSRetailPosis.Transaction.Line.LineItem)(retailTransaction.CurrentSaleLineItem)).Description == "Issue gift card")
                        {
                            isIssueToGC = 1;
                        }
                    }
                }
            }
            if (isIssueToGC == 0)
            {
                #region Nim
                if (custTrans != null)
                {
                    iIsConvertToAdv = 0;//re assign
                    if ((PosisOperations)posOperation == PosisOperations.PayCreditMemo)
                    {
                        preTriggerResult.ContinueOperation = false;
                        return;
                    }

                    SqlConnection connection = new SqlConnection();

                    if (application != null)
                        connection = application.Settings.Database.Connection;
                    else
                        connection = ApplicationSettings.Database.LocalConnection;

                    decimal dFixedRateCustOrdPercentage = Convert.ToDecimal(custTrans.PartnerData.FixedRateCustOrdPercentage);
                    decimal dFixedRateCustOrdTotAmt = Convert.ToDecimal(custTrans.PartnerData.FixedRateCustOrdTotAmt);
                    decimal dPayAmt = Convert.ToDecimal(custTrans.Amount);

                    decimal dRateFreezeAmt = (dFixedRateCustOrdTotAmt * dFixedRateCustOrdPercentage) / 100;

                    Enums.EnumClass oEnum = new Enums.EnumClass();
                    string sMaxAmount = string.Empty;
                    string sTerminalID = ApplicationSettings.Terminal.TerminalId;
                    string sMinAmt = "0";
                    bool IsRepair = false;
                    string sCustOrder = OrderNum(out IsRepair);
                    int isSV = isSuvarnaVrudhi(sCustOrder);
                    decimal dMaxSvAmt = 0m;
                    decimal dSVTollarenceAmt = 0m;
                    decimal dAdvTaxPerc = 0m;

                    decimal dGoldBookingRatePercentage = GetGoldBookingRatePer(ref dAdvTaxPerc, ref dSVTollarenceAmt);

                    //=============================Soutik
                    bool IsCustomerAdvBookQtyExists = false;

                    if (Convert.ToDecimal(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.CustAdvCommitedQty) > 0)         
                   //if (Convert.ToDecimal(retailTransaction.PartnerData.CustAdvCommitedQty) > 0)
                    {
                        IsCustomerAdvBookQtyExists = true;
                    }

                    if (IsCustomerAdvBookQtyExists)
                           sMinAmt = Convert.ToString(oEnum.CustomerAdvValidateMinDeposit(connection, out sMaxAmount, sTerminalID, Convert.ToDecimal((((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction))).CustomerDepositItem.Amount))); 
                    else if (isSV == 0)
                        sMinAmt = Convert.ToString(oEnum.ValidateMinDeposit(connection, out sMaxAmount, sTerminalID, Convert.ToDecimal((((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction))).CustomerDepositItem.Amount)));
                    else
                    {
                        getOrderAdvanceAmount(sCustOrder, ref dMaxSvAmt);

                        sMinAmt = Convert.ToString(dMaxSvAmt - dSVTollarenceAmt);
                        sMaxAmount = Convert.ToString(dMaxSvAmt);
                    }

                    if (Convert.ToDecimal(sMinAmt) != 0 && Convert.ToDecimal(sMaxAmount) != 0)
                    {
                        if (Convert.ToDecimal(sMinAmt) > Convert.ToDecimal((((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction))).CustomerDepositItem.Amount)
                            || Convert.ToDecimal(sMaxAmount) < Convert.ToDecimal((((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction))).CustomerDepositItem.Amount))
                        {
                            preTriggerResult.ContinueOperation = false;
                            preTriggerResult.MessageId = 50448;
                            return;
                        }
                        else
                        {
                            //if(dPayAmt >= dRateFreezeAmt)
                            //{
                            //    custTrans.PartnerData.GoldFixing = false;
                            //}
                            //else
                            //{
                            custTrans.PartnerData.GoldFixing = true;
                            //}
                        }
                    }
                    else
                    {
                        if (custTrans.PartnerData.OperationType != "GSS")//changed on 08/08/2017
                            custTrans.PartnerData.GoldFixing = true;
                    }


                    string sVouAgainst = Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.VOUCHERAGAINST);

                    if (!string.IsNullOrEmpty(sVouAgainst))
                    {
                        decimal dValidAmt = getNetAmount(sVouAgainst);

                        if (Convert.ToDecimal((((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction))).CustomerDepositItem.Amount) > dValidAmt)
                        {
                            preTriggerResult.ContinueOperation = false;
                            preTriggerResult.MessageId = 50448;
                            return;
                        }
                    }
                }

                //start : RH on 05/11/2014
                if (retailTransaction != null)
                {
                    if (retailTransaction.SalesInvoiceAmounts > 0)
                    {
                        return;
                    }

                    foreach (SaleLineItem saleLineItem in retailTransaction.SaleItems)
                    {
                        if (retailTransaction.IncomeExpenseAmounts == 0 && !retailTransaction.SaleIsReturnSale)//Issue gift card//&& LSRetailPosis.Transaction.Line.LineItem.
                        {
                            decimal dTotAmt = 0m;
                            if (!string.IsNullOrEmpty(saleLineItem.PartnerData.TotalAmount))
                                dTotAmt = Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);

                            if (Convert.ToDecimal(saleLineItem.PartnerData.OGFINALAMT) == 0 && dTotAmt == 0)
                            {
                                if (saleLineItem.ItemType == LSRetailPosis.Transaction.Line.GiftCertificateItem.GiftCertificateItem.ItemTypes.Item)
                                {
                                    return;
                                }
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(retailTransaction.Customer.CustomerId) && retailTransaction.IncomeExpenseAmounts == 0)
                    {
                        preTriggerResult.ContinueOperation = false;
                        MessageBox.Show("Customer selection is mandatory for the transaction.");
                        return;
                    }

                    //if(retailTransaction.IncomeExpenseAmounts != 0)
                    //{
                    //    string sTblName = "NEGCASHPAY" + ApplicationSettings.Terminal.TerminalId;
                    //    TempTableCreate(sTblName);
                    //    UpdateNegCashIfo(sTblName, ApplicationSettings.Terminal.TerminalId, 0, 1);
                    //}

                    if (retailTransaction.IncomeExpenseAmounts > 0 && string.IsNullOrEmpty(retailTransaction.PartnerData.ExpRepairBatchId))
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Is this payment for repair?", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            if (Convert.ToString(dialog.DialogResult).ToUpper().Trim() == "YES")
                            {
                                BlankOperations.WinFormsTouch.frmRepairBatchId objRBI = new BlankOperations.WinFormsTouch.frmRepairBatchId();
                                objRBI.ShowDialog();

                                string sBatchId = objRBI.sRepairBatchId;
                                retailTransaction.PartnerData.ExpRepairBatchId = sBatchId;
                            }
                        }
                    }

                    string sAdjustmentId = AdjustmentItemID();
                    dGoldBookingRatePercentage = GetGoldBookingRatePer();

                    int iPM = 100;
                    int iCF = 100;

                    foreach (SaleLineItem saleLineItem in retailTransaction.SaleItems)
                    {
                        if (!saleLineItem.Voided && retailTransaction.IncomeExpenseAmounts == 0)
                            //&& saleLineItem.ItemType != LSRetailPosis.Transaction.Line.GiftCertificateItem.GiftCertificateItem.ItemTypes.Item)
                        {
                            iIsConvertToAdv = saleLineItem.PartnerData.TransReturnPayMode;
                            if (iIsConvertToAdv == 3)
                            {
                                retailTransaction.PartnerData.GoldBookingRatePercentage = Convert.ToString(dGoldBookingRatePercentage);
                                break;
                            }

                            #region for Free gift validation
                            if (!string.IsNullOrEmpty(retailTransaction.PartnerData.FREEGIFTCON))
                            {
                                SaleLineItem saleLine1 = (SaleLineItem)saleLineItem;
                                decimal dTotFreeQtyApplied = 0m;
                                bool bFullFreeGiftApplid = false;

                                foreach (SaleLineItem saleLineItem1 in retailTransaction.SaleItems)
                                {
                                    if (!saleLineItem1.Voided)
                                    {
                                        if (saleLineItem1.NetAmount == 0)
                                            dTotFreeQtyApplied += decimal.Round((Convert.ToDecimal(saleLineItem1.PartnerData.Quantity)), 3, MidpointRounding.AwayFromZero);
                                    }
                                }

                                // dTotFreeQtyApplied = dTotFreeQtyApplied + decimal.Round((Convert.ToDecimal(saleLine.PartnerData.Quantity)), 3, MidpointRounding.AwayFromZero);

                                if (!string.IsNullOrEmpty(retailTransaction.PartnerData.FreeGiftQTY))
                                {
                                    if (dTotFreeQtyApplied != Convert.ToDecimal(retailTransaction.PartnerData.FreeGiftQTY))
                                        bFullFreeGiftApplid = false;
                                    else
                                        bFullFreeGiftApplid = true;
                                }

                                if (!bFullFreeGiftApplid)
                                {
                                    MessageBox.Show("Free quantity mismatch, alloted " + Convert.ToString(retailTransaction.PartnerData.FreeGiftQTY) + "");
                                    preTriggerResult.ContinueOperation = false;
                                    return;
                                }
                            }
                            #endregion

                        }
                    }

                    foreach (SaleLineItem saleLineItem in retailTransaction.SaleItems)
                    {
                        if (!saleLineItem.Voided)
                        {
                            iPM = getMetalType(saleLineItem.ItemId);
                            if (iPM == (int)MetalType.PackingMaterial)
                                break;
                        }
                        if (retailTransaction.IncomeExpenseAmounts == 0 && retailTransaction.SaleIsReturnSale == false && retailTransaction.SalesInvoiceAmounts == 0)
                        {
                            //if (saleLineItem.ItemType != LSRetailPosis.Transaction.Line.GiftCertificateItem.GiftCertificateItem.ItemTypes.Item)
                            //{
                                if (saleLineItem.PartnerData.TransactionType == "3"
                                    || saleLineItem.PartnerData.TransactionType == "0")
                                {
                                    string sTblName = "NEGCASHPAY" + ApplicationSettings.Terminal.TerminalId;
                                    TempTableCreate(sTblName);
                                    UpdateNegCashIfo(sTblName, ApplicationSettings.Terminal.TerminalId, 0, 1);
                                }

                                if (saleLineItem.PartnerData.TransactionType == "1"
                                    || saleLineItem.PartnerData.TransactionType == "3")//1=og purchase,3=og exchange
                                {
                                    string sOGPTblName = "OGPCASHPAY" + ApplicationSettings.Terminal.TerminalId;
                                    TempTableCreate(sOGPTblName);
                                    UpdateNegCashIfo(sOGPTblName, ApplicationSettings.Terminal.TerminalId, 0, 2);//2 for OGP
                                    UpdateTransCashInfo(sOGPTblName, ApplicationSettings.Terminal.TerminalId, retailTransaction.Customer.CustomerId);
                                }
                            //}
                        }
                        if (retailTransaction.IncomeExpenseAmounts == 0 && retailTransaction.SaleIsReturnSale == true && retailTransaction.SalesInvoiceAmounts == 0)
                        {
                            //if (saleLineItem.ItemType != LSRetailPosis.Transaction.Line.GiftCertificateItem.GiftCertificateItem.ItemTypes.Item)
                            //{
                                if (saleLineItem.PartnerData.TransReturnPayMode == 1) // SR Exchange then cash pay limit will work
                                {
                                    string sTblName = "NEGCASHPAY" + ApplicationSettings.Terminal.TerminalId;
                                    TempTableCreate(sTblName);
                                    UpdateNegCashIfo(sTblName, ApplicationSettings.Terminal.TerminalId, 0, 1);
                                }
                           // }
                        }
                    }

                    #region rechecking for same item return added on 20/07/2017
                    if (retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided
                        && retailTransaction.SaleItems.Count > 0)
                    {
                        //foreach (SaleLineItem saleLineItem1 in retailTransaction.SaleItems)
                        //{
                        if (retailTransaction.IncomeExpenseAmounts == 0 && retailTransaction.SalesInvoiceAmounts == 0)
                        {
                            string sSKU = string.Empty;
                            DataTable dt = new DataTable();
                            dt.Columns.Add("ItemId");
                            DataRow row = null;

                            var query = from i in retailTransaction.SaleItems
                                        orderby i.ItemId
                                        where !i.Voided
                                        select new { i.ItemId };

                            foreach (var rowObj in query)
                            {
                                row = dt.NewRow();

                                bool isSKU = isRetailItem(rowObj.ItemId);
                                if (isSKU)
                                {
                                    dt.Rows.Add(rowObj.ItemId);
                                }
                            }
                            
                            var duplicates = dt.AsEnumerable().GroupBy(r => r[0]).Where(gr => gr.Count() > 1).ToList();


                             if (duplicates.Any())
                            {
                                foreach (var discLine in duplicates)
                                {
                                    if (!string.IsNullOrEmpty(sSKU))
                                        sSKU = sSKU + "; " + discLine.Key.ToString();
                                    else
                                        sSKU = discLine.Key.ToString();
                                }
                                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Duplicate SKU " + sSKU + " is found in sales/sales return line ", MessageBoxButtons.OK, MessageBoxIcon.Information))
                                {
                                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                    preTriggerResult.ContinueOperation = false;
                                    return;
                                }
                            }
                        }
                        //}
                    }
                    #endregion
                    //if(retailTransaction.SaleIsReturnSale)
                    //{
                    //    string sTblName = "NEGCASHPAY" + ApplicationSettings.Terminal.TerminalId;
                    //    TempTableCreate(sTblName);
                    //    UpdateNegCashIfo(sTblName, ApplicationSettings.Terminal.TerminalId, 0, 1);
                    //}



                    if (retailTransaction != null)
                    {
                        foreach (SaleLineItem saleLineItem in retailTransaction.SaleItems)
                        {
                            if (!retailTransaction.SaleIsReturnSale && retailTransaction.IncomeExpenseAmounts == 0)
                            {
                                if (((LSRetailPosis.Transaction.Line.LineItem)(retailTransaction.CurrentSaleLineItem)).Description != "Issue gift card")
                                {
                                    if (Convert.ToDecimal(saleLineItem.PartnerData.OGFINALAMT) != 0)
                                    {
                                        if (saleLineItem.Transaction.IncomeExpenseAmounts == 0 && saleLineItem.PartnerData.TransactionType == "1")
                                        {
                                            bIsOGPurchase = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    foreach (SaleLineItem SLineItem in retailTransaction.SaleItems)
                    {
                        if (SLineItem.ItemType != BaseSaleItem.ItemTypes.Service)
                        {
                            #region If excahnge then 1 salable item should be in sales line
                            if (retailTransaction.IncomeExpenseAmounts == 0 && retailTransaction.SaleIsReturnSale == true
                                && retailTransaction.SalesInvoiceAmounts == 0)
                            {
                                int iExistSalableItem1 = 0;
                                if (SLineItem.PartnerData.isFullReturn == false)// if exchange
                                {
                                    foreach (SaleLineItem saleLineItem1 in retailTransaction.SaleItems)
                                    {
                                        if (saleLineItem1.PartnerData.TransactionType == "0" && saleLineItem1.PartnerData.NimReturnLine == 0
                                            && saleLineItem1.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service
                                            && Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount) > 0
                                            && !saleLineItem1.Voided)
                                        {
                                            iExistSalableItem1 = 1;
                                            break;
                                        }
                                    }

                                    if (iExistSalableItem1 == 0)
                                    {
                                        MessageBox.Show("Should be a salable item in sales line.", "Should be a salable item in sales line .", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        preTriggerResult.ContinueOperation = false;
                                        return;
                                    }
                                }
                            }
                            #endregion


                            #region If excahnge then  For Singapur only
                            if (retailTransaction.IncomeExpenseAmounts == 0
                                && retailTransaction.SalesInvoiceAmounts == 0)
                            {
                                if (SLineItem.PartnerData.isFullReturn == false)// if exchange
                                {
                                    bool bIsOGTaxApplicable = chkOGTaxApplicable();
                                    if (bIsOGTaxApplicable)
                                    {
                                        decimal dTotOgTaxAmt = 0m;
                                        decimal dTotSalesGoldTaxAmt = 0m;

                                        foreach (SaleLineItem saleLineItem1 in retailTransaction.SaleItems)
                                        {
                                            if (saleLineItem1.PartnerData.TransactionType == "0" && saleLineItem1.PartnerData.NimReturnLine == 0
                                                && saleLineItem1.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service
                                                && Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount) > 0
                                                && !saleLineItem1.Voided)
                                            {
                                                dTotSalesGoldTaxAmt += Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.LINEGOLDTAX));
                                            }
                                            if ((saleLineItem1.PartnerData.TransactionType == "3" || saleLineItem1.PartnerData.TransactionType == "1")
                                                && saleLineItem1.PartnerData.NimReturnLine == 0
                                                && saleLineItem1.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service
                                                && Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount) < 0
                                                && !saleLineItem1.Voided)
                                            {
                                                //dTotOgTaxAmt += Math.Abs(Convert.ToDecimal(saleLineItem1.TaxAmount));
                                                iPM = getMetalType(saleLineItem1.ItemId);
                                                if (iPM == (int)MetalType.Gold)
                                                {
                                                    dTotOgTaxAmt += Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount));
                                                }
                                            }
                                        }

                                        if (dTotOgTaxAmt > 0 && dTotSalesGoldTaxAmt > 0 && retailTransaction.PartnerData.SingaporeTaxCal == "0")
                                        {
                                            MessageBox.Show("Please enter Tax recalculate button.", "Please enter Tax recalculate button.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            preTriggerResult.ContinueOperation = false;
                                            return;
                                        }
                                    }
                                }
                            }
                            #endregion

                            #region Sales price can not less than cost price
                            string sSqr = "select isnull(NOCOSTVALIDATERETAIL,0) from INVENTTABLE where itemid='" + SLineItem.ItemId + "'";
                            string sResultForNOCOSTVALIDATERETAIL = NIM_ReturnExecuteScalar(sSqr);

                            if (sResultForNOCOSTVALIDATERETAIL == "0")
                            {
                                if (!SLineItem.Voided && retailTransaction.IncomeExpenseAmounts == 0 && retailTransaction.SaleIsReturnSale == false && retailTransaction.SalesInvoiceAmounts == 0)
                                {
                                    if (!string.IsNullOrEmpty(Convert.ToString(SLineItem.PartnerData.TotalAmount)) && Convert.ToString(SLineItem.PartnerData.TransactionType) == "0")
                                    {
                                        //dSalesValue = Convert.ToDecimal(SLineItem.PartnerData.TotalAmount);
                                        decimal dGoldValue = 0m;
                                        if (!string.IsNullOrEmpty(SLineItem.PartnerData.LINEGOLDVALUE))
                                            dGoldValue = Convert.ToDecimal(SLineItem.PartnerData.LINEGOLDVALUE);

                                        if (!getCostPrice(SLineItem.ItemId, SLineItem.NetAmountWithNoTax - dGoldValue) && SLineItem.NetAmountWithNoTax >= 0) //
                                        {
                                            MessageBox.Show("Sales price can not be less than cost price for " + SLineItem.ItemId + " item.", "Sales price can not be less than cost price.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            preTriggerResult.ContinueOperation = false;
                                            return;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (!SLineItem.Voided && retailTransaction.IncomeExpenseAmounts == 0
                                    && retailTransaction.SaleIsReturnSale == false
                                    && retailTransaction.SalesInvoiceAmounts == 0)
                                {
                                    if (!string.IsNullOrEmpty(Convert.ToString(SLineItem.PartnerData.TotalAmount)) && Convert.ToString(SLineItem.PartnerData.TransactionType) == "0")
                                    {
                                        //dSalesValue = Convert.ToDecimal(SLineItem.PartnerData.TotalAmount);
                                        decimal dGoldValue = 0m;
                                        if (!string.IsNullOrEmpty(SLineItem.PartnerData.LINEGOLDVALUE))
                                            dGoldValue = Convert.ToDecimal(SLineItem.PartnerData.LINEGOLDVALUE);

                                        if (SLineItem.NetAmountWithNoTax < dGoldValue) //
                                        {
                                            MessageBox.Show("Sales price can not be less than metal rate value for " + SLineItem.ItemId + " item.", "Sales price can not be less than metal rate value.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            preTriggerResult.ContinueOperation = false;
                                            return;
                                        }
                                    }
                                }
                            }
                            #endregion
                            // decimal dSalesValue = 0m;

                            #region final checking SKU is available or not //added on 050118
                            if (!SLineItem.Voided && retailTransaction.IncomeExpenseAmounts == 0
                                && retailTransaction.SaleIsReturnSale == false
                                && retailTransaction.SalesInvoiceAmounts == 0
                                && !Convert.ToBoolean(retailTransaction.PartnerData.SKUBookedItems))//if (!Convert.ToBoolean(retailTrans.PartnerData.SKUBookedItems))
                            {
                                bool isSKU = isRetailItem(SLineItem.ItemId);
                                if (isSKU)
                                {
                                    if (!isItemIsAvailable(SLineItem.ItemId))
                                    {
                                        MessageBox.Show("SKU " + SLineItem.ItemId + " stock not available.", "Stock issue.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        preTriggerResult.ContinueOperation = false;
                                        return;
                                    }
                                }
                            }

                            if (retailTransaction.SaleIsReturnSale)
                            {
                                if (SLineItem.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service && !SLineItem.Voided)
                                {
                                    string sReceiptId = Convert.ToString(SLineItem.PartnerData.ReturnReceiptId);
                                    string sItemId = SLineItem.ItemId;
                                    int iLine = Convert.ToInt16(SLineItem.PartnerData.NimReturnLine);
                                    string sTransId = SLineItem.ReturnTransId;
                                    string sStore = SLineItem.ReturnStoreId;
                                    string sTerminal = SLineItem.ReturnTerminalId;

                                    bool bResult = false;
                                    ReadOnlyCollection<object> containerArraySR;

                                    if (!string.IsNullOrEmpty(sReceiptId))
                                    {
                                        try
                                        {
                                            if (this.Application.TransactionServices.CheckConnection())
                                            {
                                                containerArraySR = this.Application.TransactionServices.InvokeExtension("CheckItemIsReturn", sReceiptId, iLine);//sItemId,
                                                bResult = Convert.ToBoolean(containerArraySR[1]);
                                            }
                                            else
                                            {
                                                bResult = isItemIsAlreadyReturn(sTransId, sStore, sTerminal, iLine);
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            bResult = isItemIsAlreadyReturn(sTransId, sStore, sTerminal, iLine);
                                        }
                                        if (bResult == false)
                                        {
                                            bResult = isItemIsAlreadyReturn(sTransId, sStore, sTerminal, iLine);
                                        }


                                        if (bResult == true)
                                        {
                                            MessageBox.Show(string.Format("Item " + sItemId + " is already returned."));
                                            preTriggerResult.ContinueOperation = false;
                                            return;
                                        }
                                    }
                                }

                            }

                            #endregion

                        }
                    }

                    //==================================Soutik
                    #region Check Customer Deposit Commited Qty Greater Than Sale Qty  // Added On 30-07-2019
                    decimal dcommitedQty = 0;
                    int iTotalNoOfAdvance = 0;
                    if (retailTransaction != null && retailTransaction.SaleItems.Count > 0 && retailTransaction.IncomeExpenseAmounts == 0)
                    {
                        foreach (SaleLineItem SLineItem in retailTransaction.SaleItems)
                        {
                            if (SLineItem.ItemId == sAdjustmentId)
                            {
                                dcommitedQty = Convert.ToDecimal(retailTransaction.PartnerData.CustAdvCommitedQty);
                                iTotalNoOfAdvance += 1;
                            }
                            else
                            {
                                dSaleQty += decimal.Round((Convert.ToDecimal(SLineItem.PartnerData.Quantity)), 3, MidpointRounding.AwayFromZero);
                            }
                        }
                        if (dcommitedQty > dSaleQty && dcommitedQty > 0)
                        {
                            MessageBox.Show("Commited quantity must not be greater than sale quantity ");
                            preTriggerResult.ContinueOperation = false;
                            return;
                        }

                        if (iTotalNoOfAdvance > 1 && dcommitedQty > 0)
                        {
                            MessageBox.Show("When Commited quantity exists then no more advance adjusted ");
                            preTriggerResult.ContinueOperation = false;
                            return;
                        }

                    }

                    #endregion


                    #region iIsAdvAdjExist && iSalesItemExist
                    int iIsAdvAdjExist = 0;

                    bool bIsSale = false;
                    if (retailTransaction != null
                      && retailTransaction.IncomeExpenseAmounts == 0
                      && retailTransaction.PartnerData.LOYALTYASKINGDONE == false
                        && retailTransaction.SaleItems.Count > 0)
                    {
                        foreach (SaleLineItem SLineItem in retailTransaction.SaleItems)
                        {
                            //if (SLineItem.ItemType != LSRetailPosis.Transaction.Line.GiftCertificateItem.GiftCertificateItem.ItemTypes.Item)
                            //{
                            if (SLineItem.PartnerData.TransactionType == "0"
                                    && SLineItem.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service
                                    && Convert.ToDecimal(SLineItem.PartnerData.TotalAmount) > 0
                                    && !SLineItem.Voided)
                            {
                                bIsSale = true;
                                break;
                            }
                            //}
                        }
                    }

                    foreach (SaleLineItem SLineItem in retailTransaction.SaleItems)
                    {
                        if (retailTransaction.SaleItems.Count > 0)
                        {
                            if (SLineItem.ItemId == sAdjustmentId)
                            {
                                iIsAdvAdjExist = 1;
                                break;
                            }
                        }
                    }

                    if (iIsAdvAdjExist == 1 && bIsSale == false)
                    {
                        retailTransaction.RefundReceiptId = "1";
                    }
                    #endregion

                    int iExistSalableItem = 0;
                    if (bIsSale)
                        iExistSalableItem = 1;

                    if (retailTransaction.TransSalePmtDiff < 0
                        && retailTransaction.IncomeExpenseAmounts == 0
                        && retailTransaction.SaleIsReturnSale == false)
                    {
                        #region Commented
                        //foreach (SaleLineItem saleLineItem in retailTransaction.SaleItems)
                        //{
                        //    if (saleLineItem.PartnerData.TransactionType == "0"
                        //        && saleLineItem.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service
                        //        && Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0
                        //        && !saleLineItem.Voided)
                        //    {
                        //        iExistSalableItem = 1;
                        //        break;
                        //    }
                        //    if (saleLineItem.ItemId != sAdjustmentId)
                        //    {
                        //        iExistSalableItem = 1;
                        //        break;
                        //    }
                        //}
                        #endregion
                        bool bIsSaleReturnToAdvance = isSRToAdvance(Convert.ToInt16(tenderId));
                        int iSPId = 0;
                        iSPId = getUserDiscountPermissionId();

                        if (Convert.ToInt16(iSPId) != (int)CRWRetailDiscPermission.Manager2)//Convert.ToInt16(iSPId) != (int)CRWRetailDiscPermission.Manager  && 
                        {
                            if (iExistSalableItem == 0 && !bIsSaleReturnToAdvance)
                            {
                                MessageBox.Show("Should be a salable item in sales line.Cashback not applicable for this user", "Should be a salable item in sales line.Cashback not applicable for this user .", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                preTriggerResult.ContinueOperation = false;
                                return;
                            }
                        }
                    }




                    if (retailTransaction != null
                     && retailTransaction.IncomeExpenseAmounts == 0
                     && retailTransaction.PartnerData.LOYALTYASKINGDONE == false
                       && retailTransaction.SaleItems.Count > 0)
                    {
                        foreach (SaleLineItem SLineItem in retailTransaction.SaleItems)
                        {
                            if (!retailTransaction.SaleIsReturnSale)
                            {

                                if (Convert.ToString(SLineItem.PartnerData.LOYALTYTYPECODE) != "")
                                {
                                    if (SLineItem.ItemType != LSRetailPosis.Transaction.Line.GiftCertificateItem.GiftCertificateItem.ItemTypes.Item)
                                    {
                                        string sLTC = SLineItem.PartnerData.LOYALTYTYPECODE;
                                        string sLP = SLineItem.PartnerData.LoyaltyProvider;
                                        string sLN = SLineItem.PartnerData.LOYALTYCARDNO;

                                        retailTransaction.PartnerData.LOYALTYASKINGDONE = true;

                                        retailTransaction.PartnerData.LOYALTYTYPECODE = sLTC;
                                        retailTransaction.PartnerData.LoyaltyProvider = sLP;
                                        retailTransaction.PartnerData.LOYALTYCARDNO = sLN;
                                        bIsSale = true;
                                        break;
                                    }
                                }

                            }
                        }
                    }


                    #region Print Cust Name
                    if (retailTransaction != null
                        && bIsSale == true
                        && retailTransaction.EntryStatus != LSRetailPosis.Transaction.PosTransaction.TransactionStatus.OnHold)
                    {
                        if (string.IsNullOrEmpty(Convert.ToString(retailTransaction.PartnerData.LCCustomerName)))
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Do you want enter another print name? ", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                if (Convert.ToString(dialog.DialogResult).ToUpper().Trim() == "YES")
                                {
                                    Interaction.frmInput objI = new Interaction.frmInput();
                                    objI.ShowDialog();

                                    if (!string.IsNullOrEmpty(objI.InputText))
                                    {
                                        retailTransaction.PartnerData.LCCustomerName = objI.InputText;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Print name is mandatory.", "Print name is mandatory .", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        preTriggerResult.ContinueOperation = false;
                                        return;
                                    }

                                }
                            }
                        }
                    }


                    #endregion



                    #region
                    //Start : pay amount 11 digit validation added on 10102017
                    /*if (retailTransaction != null )
                    {
                        if (retailTransaction.TransSalePmtDiff != 0)
                        {
                            string sPayAmt = Convert.ToString(retailTransaction.TransSalePmtDiff);
                            if (sPayAmt.Length > 11)
                            {
                                preTriggerResult.ContinueOperation = false;
                                MessageBox.Show("Invalid paying amount.");
                                return;
                            }
                        }
                    }*/
                    //End 

                    // retailTrans != null && bIsSale == 1 && retailTrans.EntryStatus != LSRetailPosis.Transaction.PosTransaction.TransactionStatus.OnHold
                    /*if (retailTransaction != null && bIsSale == true
                        && retailTransaction.PartnerData.LOYALTYASKINGDONE == false
                        && retailTransaction.EntryStatus != LSRetailPosis.Transaction.PosTransaction.TransactionStatus.OnHold)
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Do you have any third party Loyalty Card ?", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            retailTransaction.PartnerData.LOYALTYASKINGDONE = true;
                            if (Convert.ToString(dialog.DialogResult).ToUpper().Trim() == "YES")
                            {
                                string sLTC = string.Empty;
                                string sLP = string.Empty;
                                string sLN = string.Empty;
                                DataTable dtLTCode = new DataTable();
                                dtLTCode = GetAdvDetailInfo(" select loyaltyTypeCode from CRWThirdPartyLoyaltymaster");

                                DataRow row = dtLTCode.NewRow();
                                dtLTCode.Rows.InsertAt(row, 0);


                                frmThirdPartyLoyaltyCard frmTPL = new frmThirdPartyLoyaltyCard(dtLTCode);
                                frmTPL.ShowDialog();

                                SqlConnection conn = new SqlConnection();
                                if (application != null)
                                    conn = application.Settings.Database.Connection;
                                else
                                    conn = ApplicationSettings.Database.LocalConnection;

                                sLTC = frmTPL.sTypeCode;
                                sLP = frmTPL.sProvider;
                                sLN = frmTPL.sCardNo;

                                if (!string.IsNullOrEmpty(sLTC))//LOYALTYTYPECODE,LoyaltyProvider,LOYALTYCARDNO
                                {
                                    retailTransaction.PartnerData.LOYALTYTYPECODE = sLTC;
                                    retailTransaction.PartnerData.LoyaltyProvider = sLP;
                                    retailTransaction.PartnerData.LOYALTYCARDNO = sLN;
                                }
                            }
                        }
                    }*/


                    /* foreach (SaleLineItem saleLineItem in retailTransaction.SaleItems)
                     {
                         if (!saleLineItem.Voided)
                         {
                             iCF = getMetalType(saleLineItem.ItemId);
                             if (iCF == (int)MetalType.Certificate)
                                 break;
                         }
                     }*/
                    #endregion

                    #region Packing Material Commented
                    //foreach(SaleLineItem saleLineItem in retailTransaction.SaleItems)
                    //{
                    //    if(saleLineItem.ReturnLineId == 0)
                    //    {
                    //        if(retailTransaction.PartnerData.PackingMaterial != "Y")
                    //        {
                    //            if(IsRetailItem(saleLineItem.ItemId))
                    //            {
                    //                if(iPM != (int)MetalType.PackingMaterial)
                    //                {
                    //                    #region Commented
                    //                    //using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Have you issued packing material?", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    //                    //{
                    //                    //    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    //                    //    if(Convert.ToString(dialog.DialogResult).ToUpper().Trim() == "NO")
                    //                    //    {
                    //                    //        preTriggerResult.ContinueOperation = false;
                    //                    //        return;
                    //                    //    }
                    //                    //    else
                    //                    //    {
                    //                    //        retailTransaction.PartnerData.PackingMaterial = "Y";

                    //                    //        if(IsCertificateItem(saleLineItem.ItemId))
                    //                    //        {
                    //                    //            if(iCF != (int)MetalType.Certificate)
                    //                    //            {
                    //                    //                using(LSRetailPosis.POSProcesses.frmMessage dialog1 = new LSRetailPosis.POSProcesses.frmMessage("Have you issued the certificate?", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    //                    //                {
                    //                    //                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog1);
                    //                    //                    if(Convert.ToString(dialog1.DialogResult).ToUpper().Trim() == "NO")
                    //                    //                    {
                    //                    //                        preTriggerResult.ContinueOperation = false;
                    //                    //                        return;
                    //                    //                    }
                    //                    //                    else
                    //                    //                        retailTransaction.PartnerData.CertificateIssue = "Y";
                    //                    //                }
                    //                    //            }
                    //                    //            else
                    //                    //                retailTransaction.PartnerData.CertificateIssue = "Y";
                    //                    //        }
                    //                    //    }
                    //                    //}
                    //                    #endregion
                    //                    //using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Proceed without packing material?", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    //                    //{
                    //                    //    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    //                    //    if(Convert.ToString(dialog.DialogResult).ToUpper().Trim() == "NO")
                    //                    //    {
                    //                    //        preTriggerResult.ContinueOperation = false;
                    //                    //        return;
                    //                    //    }
                    //                    //}
                    //                }
                    //                else
                    //                {
                    //                    retailTransaction.PartnerData.PackingMaterial = "Y";

                    //                    #region Commented
                    //                    //if(IsCertificateItem(saleLineItem.ItemId))
                    //                    //{
                    //                    //    if(iCF != (int)MetalType.Certificate)
                    //                    //    {
                    //                    //        using(LSRetailPosis.POSProcesses.frmMessage dialog1 = new LSRetailPosis.POSProcesses.frmMessage("Have you issued the certificate?", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    //                    //        {
                    //                    //            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog1);
                    //                    //            if(Convert.ToString(dialog1.DialogResult).ToUpper().Trim() == "NO")
                    //                    //            {
                    //                    //                preTriggerResult.ContinueOperation = false;
                    //                    //                return;
                    //                    //            }
                    //                    //            else
                    //                    //                retailTransaction.PartnerData.CertificateIssue = "Y";
                    //                    //        }
                    //                    //    }
                    //                    //    else
                    //                    //        retailTransaction.PartnerData.CertificateIssue = "Y";
                    //                    //}
                    //                    #endregion
                    //                }
                    //            }
                    //        }
                    //        #region Commented
                    //        //    else if(retailTransaction.PartnerData.CertificateIssue != "Y")
                    //        //    {
                    //        //        if(IsCertificateItem(saleLineItem.ItemId))
                    //        //        {
                    //        //            if(iCF != (int)MetalType.Certificate)
                    //        //            {
                    //        //                using(LSRetailPosis.POSProcesses.frmMessage dialog1 = new LSRetailPosis.POSProcesses.frmMessage("Have you issued the certificate?", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    //        //                {
                    //        //                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog1);
                    //        //                    if(Convert.ToString(dialog1.DialogResult).ToUpper().Trim() == "NO")
                    //        //                    {
                    //        //                        preTriggerResult.ContinueOperation = false;
                    //        //                        return;
                    //        //                    }
                    //        //                    else
                    //        //                        retailTransaction.PartnerData.CertificateIssue = "Y";
                    //        //                }
                    //        //            }
                    //        //            else
                    //        //                retailTransaction.PartnerData.CertificateIssue = "Y";
                    //        //        }
                    //        //    }
                    //        #endregion
                    //    }
                    //}
                    #endregion
                }
                // end: RH on 05/11/2014


                bool bIsSRToAdv = isSRToAdvance(Convert.ToInt16(tenderId));

                bool bIsLoayltyPay = isLoyaltyPayment(Convert.ToInt16(tenderId));

                bool bIsRoundOffMOP = isRoundOff(Convert.ToInt16(tenderId));

                if (bIsRoundOffMOP)
                {
                    decimal dParamLimit = GetRoundOffMOPLimit();
                    if (retailTransaction.TransSalePmtDiff > dParamLimit)
                    {
                        preTriggerResult.ContinueOperation = false;
                        MessageBox.Show(string.Format("Round off transaction limit is Rs. " + dParamLimit + ""));
                        return;
                    }
                    else
                    {
                        if (retailTransaction.TransSalePmtDiff < 0)
                        {
                            MessageBox.Show(string.Format("Invalid MOP selected."));
                            preTriggerResult.ContinueOperation = false;
                            return;
                        }
                    }
                }

                if (bIsLoayltyPay)
                {
                    if (retailTransaction != null)
                    {
                        if (retailTransaction.Customer != null)
                        {
                            retailTransaction.LoyaltyItem.UsageType =
                                LSRetailPosis.Transaction.Line.LoyaltyItem.LoyaltyItemUsageType.NotUsed;
                            retailTransaction.LoyaltyItem.LoyaltyCardNumber = "";
                            retailTransaction.LoyaltyItem.LoyaltyCustID = "";
                            retailTransaction.LoyaltyItem.SchemeID = "";
                            retailTransaction.LoyaltyItem.CustID = "";
                        }
                    }
                }

                if (bIsSRToAdv)
                {
                    retailTransaction.PartnerData.GoldBookingRatePercentage = Convert.ToString(dGoldBookingRatePercentage);

                    //start: added for GSS adjustment not to advance
                    string sGSSNo = Convert.ToString(retailTransaction.PartnerData.GSSMaturityNo);
                    //if (!string.IsNullOrEmpty(sGSSNo))// commented by Vitthal on 22042019
                    //{
                    //    preTriggerResult.ContinueOperation = false;
                    //    MessageBox.Show("Covert to advance not a valid MOP for this type of transaction.");
                    //    return;
                    //}
                    //End

                    foreach (SaleLineItem saleLineItem in retailTransaction.SaleItems)
                    {
                        if (!saleLineItem.Voided)
                        {
                            if (retailTransaction.TransSalePmtDiff > 0)
                            {
                                preTriggerResult.ContinueOperation = false;
                                MessageBox.Show("Covert to advance not a valid MOP for this type of transaction.");
                                return;
                            }

                            if (saleLineItem.PartnerData.TransReturnPayMode == 2)//Cash back @ SR
                            {
                                preTriggerResult.ContinueOperation = false;
                                MessageBox.Show("Covert to advance not a valid MOP for this type of transaction.");
                                return;
                            }
                        }
                    }
                    frmOptionSelectionGSSorCustomerOrder objOption = new frmOptionSelectionGSSorCustomerOrder(posTransaction, 2);
                    objOption.ShowDialog();

                    if (objOption.isGSS == false)
                    {
                        DataRow drCounter = null;
                        string ssQl = " SELECT ORDERNUM AS [ORDER NO],ORDERDATE AS [ORDER DATE],DELIVERYDATE AS [DELIVERY DATE], " +
                                     " CUSTNAME AS [CUSTOMER NAME],TOTALAMOUNT AS [TOTAL AMOUNT]  FROM CUSTORDER_HEADER  " +
                                     " WHERE isConfirmed=1 AND isDelivered=0 AND GMA=0 AND CUSTACCOUNT='" + retailTransaction.Customer.CustomerId + "'" +
                                     " ORDER BY ORDERNUM ASC";
                        DataTable dt = GetDataTableInfo(ssQl);// common method for getting datatable

                        Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch oSearch = new Dialog.WinFormsTouch.frmGenericSearch(dt, drCounter = null, "Order selection");
                        oSearch.ShowDialog();
                        drCounter = oSearch.SelectedDataRow;
                        string sOrderN = "";
                        if (drCounter != null)
                        {
                            sOrderN = Convert.ToString(drCounter["ORDER NO"]);
                            retailTransaction.PartnerData.AdjustmentOrderNum = sOrderN;
                        }
                        else
                            retailTransaction.PartnerData.AdjustmentOrderNum = "";
                    }
                }
                else
                  {
                    if (iIsConvertToAdv == 3)
                    {
                        MessageBox.Show("Payment mode should be convert to advance.", "Payment mode should be convert to advance.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        preTriggerResult.ContinueOperation = false;
                        return;
                    }
                }
                #endregion
            }
            //End:Nim

            LSRetailPosis.ApplicationLog.Log("PaymentTriggers.PrePayment", "On the start of a payment operation...", LSRetailPosis.LogTraceLevel.Trace);

            switch ((PosisOperations)posOperation)
            {
                case PosisOperations.PayCash:
                    // Insert code here...
                    //Start:Nim
                    if (retailTransaction != null)
                    {
                        if (retailTransaction.IncomeExpenseAmounts == 0)
                        {
                            decimal dTodaysPaidCash = GetTodaysTotCashPaymentForSelectedCustomer(retailTransaction, null);
                            if (retailTransaction.TransSalePmtDiff > 0)
                            {
                                string sTableName = "DAILYCASHPAY" + ApplicationSettings.Terminal.TerminalId;
                                tableCreate(sTableName);

                                UpdateDailyCashIfo(sTableName, ApplicationSettings.Terminal.TerminalId, dTodaysPaidCash);
                            }
                        }

                        if (retailTransaction.IncomeExpenseAmounts == 0)
                        {
                            decimal dTodaysPaidCash = GetTodaysTotCashPaymentForSelectedCustomerNegetive(retailTransaction, null);
                            if (retailTransaction.TransSalePmtDiff < 0)
                            {
                                string sTableName = "DAILYCASHPAYNEG" + ApplicationSettings.Terminal.TerminalId;
                                tableCreate(sTableName);

                                UpdateDailyCashIfo(sTableName, ApplicationSettings.Terminal.TerminalId, dTodaysPaidCash);
                            }
                        }

                        string sGSSNo = Convert.ToString(retailTransaction.PartnerData.GSSMaturityNo);
                        if (!string.IsNullOrEmpty(sGSSNo) && retailTransaction.TransSalePmtDiff < 0)
                        {
                            decimal dParamLimit = GetGSSMaxCashLimit();
                            if ((Math.Abs(retailTransaction.AmountDue)) > dParamLimit)
                            {
                                preTriggerResult.ContinueOperation = false;
                                MessageBox.Show(string.Format("Cash transaction limit exceeding."));
                                return;
                            }
                        }

                        if (retailTransaction.IncomeExpenseAmounts < 0)
                        {
                            decimal dTodaysPaidCash = GetTodaysTotIncomeWithCash();
                            decimal dParamLimit = GetIncomeExpMaxCashLimit();
                            if ((Math.Abs(retailTransaction.IncomeExpenseAmounts) + dTodaysPaidCash) > dParamLimit)
                            {
                                preTriggerResult.ContinueOperation = false;
                                MessageBox.Show(string.Format("Cash transaction limit exceeding."));
                                return;
                            }
                        }

                        if (retailTransaction.IncomeExpenseAmounts > 0)
                        {
                            decimal dTodaysPaidCash = GetTodaysTotExpenseWithCash();
                            decimal dParamLimit = GetIncomeExpMaxCashLimit();
                            if ((Math.Abs(retailTransaction.IncomeExpenseAmounts) + dTodaysPaidCash) > dParamLimit)
                            {
                                preTriggerResult.ContinueOperation = false;
                                MessageBox.Show(string.Format("Cash transaction limit exceeding."));
                                return;
                            }
                        }

                    }

                    if (custTrans != null)
                    {
                        decimal dTodaysPaidCash = GetTodaysTotCashPaymentForSelectedCustomer(retailTransaction, custTrans);
                        if (custTrans.TransSalePmtDiff > 0)
                        {
                            string sTableName = "DAILYCASHPAY" + ApplicationSettings.Terminal.TerminalId;
                            tableCreate(sTableName);

                            UpdateDailyCashIfo(sTableName, ApplicationSettings.Terminal.TerminalId, dTodaysPaidCash);
                        }
                    }
                    //End:Nim
                    break;
                case PosisOperations.PayCard:
                    // Insert code here...
                    //Start:Nim
                    if (retailTransaction != null)
                    {
                        //if (bIsOGPurchase == true)
                        //{
                        //    if (retailTransaction.TransSalePmtDiff < 0)
                        //    {
                        //        MessageBox.Show(string.Format("Invalid MOP selected."));
                        //        preTriggerResult.ContinueOperation = false;
                        //        return;
                        //    }
                        //}
                        if (retailTransaction.TransSalePmtDiff < 0)
                        {
                            MessageBox.Show(string.Format("Invalid MOP selected."));
                            preTriggerResult.ContinueOperation = false;
                            return;
                        }

                    }
                    //End:Nim
                    break;
                case PosisOperations.PayCheque:
                    // Insert code here...
                    //Start:Nim
                    //if (retailTransaction != null)
                    //{
                    //    if (bIsOGPurchase == true)
                    //    {
                    //        preTriggerResult.ContinueOperation = false;
                    //        MessageBox.Show(string.Format("Invalid MOP selected."));
                    //        return;
                    //    }
                    //}
                    //End:Nim
                    break;
                case PosisOperations.PayCorporateCard:
                    // Insert code here...
                    //Start:Nim
                    if (retailTransaction != null)
                    {
                        //if (bIsOGPurchase == true)
                        //{
                        //    if (retailTransaction.TransSalePmtDiff < 0)
                        //    {
                        //        MessageBox.Show(string.Format("Invalid MOP selected."));
                        //        preTriggerResult.ContinueOperation = false;
                        //        return;
                        //    }
                        //}
                        if (retailTransaction.TransSalePmtDiff < 0)
                        {
                            MessageBox.Show(string.Format("Invalid MOP selected."));
                            preTriggerResult.ContinueOperation = false;
                            return;
                        }
                    }
                    break;
                case PosisOperations.PayCreditMemo:
                    // Insert code here...
                    break;
                case PosisOperations.PayCurrency:
                    // Insert code here...
                    //Start:Nim
                    if (retailTransaction != null)
                    {
                        if (bIsOGPurchase == true)
                        {
                            if (retailTransaction.TransSalePmtDiff < 0)
                            {
                                MessageBox.Show(string.Format("Invalid MOP selected."));
                                preTriggerResult.ContinueOperation = false;
                                return;
                            }
                        }
                    }
                    break;
                case PosisOperations.PayCustomerAccount:
                    // Insert code here...
                    if (retailTransaction.CurrentSaleLineItem != null)
                    {
                        if (((LSRetailPosis.Transaction.Line.LineItem)(retailTransaction.CurrentSaleLineItem)).Description == "Issue gift card")
                        {
                            preTriggerResult.ContinueOperation = false;
                            MessageBox.Show(string.Format("Invalid MOP selected."));
                            return;
                        }
                    }
                    else if (iIsConvertToAdv == 3)
                    {
                        preTriggerResult.ContinueOperation = false;
                        MessageBox.Show(string.Format("Invalid MOP selected."));
                        return;
                    }
                    break;
                case PosisOperations.PayGiftCertificate:
                    // Insert code here...
                    //Start:Nim
                    if (retailTransaction != null)
                    {
                        if (bIsOGPurchase == true)
                        {
                            if (retailTransaction.TransSalePmtDiff < 0)
                            {
                                MessageBox.Show(string.Format("Invalid MOP selected."));
                                preTriggerResult.ContinueOperation = false;
                                return;
                            }
                        }
                    }
                    break;
                case PosisOperations.PayLoyalty:
                    // Insert code here...
                    //Start:Nim
                    if (retailTransaction != null)
                    {
                        if (bIsOGPurchase == true)
                        {
                            if (retailTransaction.TransSalePmtDiff < 0)
                            {
                                MessageBox.Show(string.Format("Invalid MOP selected."));
                                preTriggerResult.ContinueOperation = false;
                                return;
                            }
                        }
                    }
                    break;

                // etc.....
            }
        }

        /// <summary>
        /// Triggered before voiding of a payment.
        /// </summary>
        /// <param name="preTriggerResult"></param>
        /// <param name="posTransaction"></param>
        /// <param name="lineId"> </param>
        public void PreVoidPayment(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction, int lineId)
        {
            LSRetailPosis.ApplicationLog.Log("PaymentTriggers.PreVoidPayment", "Before the void payment operation...", LSRetailPosis.LogTraceLevel.Trace);
        }

        /// <summary>
        /// Triggered after voiding of a payment.
        /// </summary>
        /// <param name="posTransaction"></param>
        /// <param name="lineId"> </param>
        public void PostVoidPayment(IPosTransaction posTransaction, int lineId)
        {
            LSRetailPosis.ApplicationLog.Log("PaymentTriggers.PostVoidPayment", "After the void payment operation...", LSRetailPosis.LogTraceLevel.Trace);
        }

        /// <summary>
        /// Triggered before registering cash payment.
        /// </summary>
        /// <param name="preTriggerResult"></param>
        /// <param name="posTransaction"></param>
        /// <param name="posOperation"></param>
        /// <param name="tenderId"></param>
        /// <param name="currencyCode"></param>
        /// <param name="amount"></param>
        public void PreRegisterPayment(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction, object posOperation, string tenderId, string currencyCode, decimal amount)
        {
            LSRetailPosis.ApplicationLog.Log("PaymentTriggers.PreRegisterPayment", "Before registering the payment...", LSRetailPosis.LogTraceLevel.Trace);
        }

        #endregion
    }
}
