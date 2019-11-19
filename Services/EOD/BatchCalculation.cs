/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using LSRetailPosis.DataAccess;
using LSRetailPosis.DataAccess.DataUtil;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Diagnostics;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using TransactionStatus = LSRetailPosis.Transaction.PosTransaction.TransactionStatus;
using TypeOfTransaction = LSRetailPosis.Transaction.PosTransaction.TypeOfTransaction;
using LSRetailPosis.Transaction;

namespace Microsoft.Dynamics.Retail.Pos.EOD
{
    /// <summary>
    /// Class warps Batch calculation functions.
    /// </summary>
    static class BatchCalculation
    {

        #region Fields

        private static TenderDeclarationCalculationType tenderDeclCalculationType;
        private static TenderZeroDeclarationType tenderZeroDeclarationType;

        #endregion

        #region Enums
        /// <summary>
        /// Specifies the type of Tender Declaration Calculation method to use.
        /// </summary>
        /// <remarks>Linked to AX Base enum: RetailTenderDeclarationCalculationBas.</remarks>
        public enum TenderDeclarationCalculationType
        {
            /// <summary>
            /// Last = 0
            /// </summary>
            Last = 0,

            /// <summary>
            /// Sum = 1
            /// </summary>
            Sum = 1
        }

        public enum TenderZeroDeclarationType
        {
            /// <summary>
            /// Last = 0
            /// </summary>
            ZeroAsNone = 0,

            /// <summary>
            /// Sum = 1
            /// </summary>
            ZeroAsZero = 1
        }
        #endregion

        #region Queries

        private const string sqlHeaderJoinClause = "INNER JOIN RETAILTRANSACTIONTABLE AS H " +
            "ON H.TRANSACTIONID = L.TRANSACTIONID " +
            "AND H.STORE = L.STORE " +
            "AND H.TERMINAL = L.TERMINALID " +
            "AND H.DATAAREAID = L.DATAAREAID ";

        private const string sqlHeaderJoinClause1 = "INNER JOIN RETAILTRANSACTIONTABLE AS H " +
            "ON H.TRANSACTIONID = L.TRANSACTIONID " +
            "AND H.STORE = L.STORE " +
            "AND H.TERMINAL = L.TERMINAL " +
            "AND H.DATAAREAID = L.DATAAREAID ";

        private const string sqlHeaderJoinClause2 = "INNER JOIN RETAILTRANSACTIONTABLE AS H " +
            "ON H.TRANSACTIONID = L2.TRANSACTIONID " +
            "AND H.STORE = L2.STORE " +
            "AND H.TERMINAL = L2.TERMINAL " +
            "AND H.DATAAREAID = L2.DATAAREAID ";

        private const string sqlStoreTenderJoinClause = "INNER JOIN RETAILSTORETENDERTYPETABLE AS T ON T.TENDERTYPEID = L.TENDERTYPE " +
            "AND T.DATAAREAID = L.DATAAREAID " +
            "JOIN RETAILSTORETABLE AS ST " +
            "ON ST.RECID = T.CHANNEL " +
            "AND ST.STORENUMBER = L.STORE ";

        private const string sqlStoreTenderJoinClause2 = "INNER JOIN RETAILSTORETENDERTYPETABLE AS T ON T.TENDERTYPEID = L2.TENDERTYPE " +
            "AND T.DATAAREAID = L2.DATAAREAID " +
            "JOIN RETAILSTORETABLE AS ST " +
            "ON ST.RECID = T.CHANNEL " +
            "AND ST.STORENUMBER = L2.STORE ";

        private const string sqlWhereBatchClause = "WHERE H.STORE = @STOREID " +
            "AND H.BATCHTERMINALID = @BATCHTERMINALID " +
            "AND H.DATAAREAID = @DATAAREAID " +
            "AND H.BATCHID = @BATCHID ";

        private const string sqlSalesTotal = "SELECT -(SUM(L.NETAMOUNT) + CASE WHEN @TAXINCLUSIVE=1 THEN SUM(L.TAXAMOUNT) ELSE 0 END) " +
            "FROM RETAILTRANSACTIONSALESTRANS AS L " +
            sqlHeaderJoinClause +
            sqlWhereBatchClause +
            "AND QTY < 0 " +
            "AND ((ENTRYSTATUS = @TRANSACTIONSTATUS) OR (TYPE = @TYPECUSTOMERORDER AND ENTRYSTATUS = @TRANSACTIONSTATUSFORCUSTOMERORDER)) " +
            "AND TRANSACTIONSTATUS = @TRANSACTIONSTATUS ";

        private const string sqlReturnsTotal = "SELECT SUM(L.NETAMOUNT) + CASE WHEN @TAXINCLUSIVE=1 THEN SUM(L.TAXAMOUNT) ELSE 0 END " +
            "FROM RETAILTRANSACTIONSALESTRANS L " +
            sqlHeaderJoinClause +
            sqlWhereBatchClause +
            "AND QTY > 0 " +
            "AND ((ENTRYSTATUS = @TRANSACTIONSTATUS) OR (TYPE = @TYPECUSTOMERORDER AND ENTRYSTATUS = @TRANSACTIONSTATUSFORCUSTOMERORDER)) " +
            "AND TRANSACTIONSTATUS = @TRANSACTIONSTATUS ";

        private const string sqlTaxTotal = "SELECT SUM(TAXAMOUNT) AS TAXTOTAL " +
            "FROM RETAILTRANSACTIONSALESTRANS L " +
            sqlHeaderJoinClause +
            sqlWhereBatchClause +
            "AND ((ENTRYSTATUS = @TRANSACTIONSTATUS) OR (TYPE = @TYPECUSTOMERORDER AND ENTRYSTATUS = @TRANSACTIONSTATUSFORCUSTOMERORDER)) " +
            "AND TRANSACTIONSTATUS = @TRANSACTIONSTATUS ";

        private const string sqlDiscountTotal = "SELECT SUM(L.DISCAMOUNT) " +
            "FROM RETAILTRANSACTIONSALESTRANS AS L " +
            sqlHeaderJoinClause +
            sqlWhereBatchClause +
            "AND ((ENTRYSTATUS = @TRANSACTIONSTATUS) OR (TYPE = @TYPECUSTOMERORDER AND ENTRYSTATUS = @TRANSACTIONSTATUSFORCUSTOMERORDER)) " +
            "AND TRANSACTIONSTATUS = @TRANSACTIONSTATUS";

        private const string sqlCustomersCount = "SELECT COUNT(CUSTACCOUNT) " +
            "FROM RETAILTRANSACTIONTABLE AS H " +
            "INNER JOIN RETAILSTORETABLE AS S ON H.STORE = S.STORENUMBER " +
            sqlWhereBatchClause +
            "AND ((ENTRYSTATUS = @TRANSACTIONSTATUS) OR (TYPE = @TYPECUSTOMERORDER AND ENTRYSTATUS = @TRANSACTIONSTATUSFORCUSTOMERORDER)) " +
            "AND CUSTACCOUNT <> ''";

        private const string sqlPaidToAccountTotal = "SELECT SUM(AMOUNTTENDERED) " +
            "FROM RETAILTRANSACTIONPAYMENTTRANS AS L " +
            sqlHeaderJoinClause1 +
            sqlWhereBatchClause +
            "AND TRANSACTIONSTATUS = @TRANSACTIONSTATUS " +
            "AND [TYPE] = @TRANSACTIONTYPE";

        private const string sqlSuspendedTransactonCount = "SELECT COUNT(*) " +
            "FROM RETAILSUSPENDEDTRANSACTIONS " +
            "WHERE STOREID = @STOREID " +
            "AND TERMINALID = @TERMINALID " +
            "AND DATAAREAID = @DATAAREAID ";

        private const string sqlRoundingAmountTotal = "SELECT SUM(SALESPAYMENTDIFFERENCE) " +
            "FROM RETAILTRANSACTIONTABLE AS H " +
            sqlWhereBatchClause;

        private const string sqlTransactonsCount = "SELECT COUNT(*) " +
            "FROM RETAILTRANSACTIONTABLE AS H " +
            sqlWhereBatchClause;

        private const string sqlSalesCount = "select COUNT(DISTINCT L.TRANSACTIONID) " +
            "FROM RETAILTRANSACTIONSALESTRANS AS L " +
            sqlHeaderJoinClause +
            sqlWhereBatchClause +
            "AND ((ENTRYSTATUS = @TRANSACTIONSTATUS) OR (TYPE = @TYPECUSTOMERORDER AND ENTRYSTATUS = @TRANSACTIONSTATUSFORCUSTOMERORDER)) ";

        private const string sqlVoidsCount = "SELECT COUNT(*) " +
            "FROM RETAILTRANSACTIONTABLE AS H " +
            sqlWhereBatchClause +
            "AND ENTRYSTATUS = @TRANSACTIONSTATUS ";

        private const string sqlTransactionTypesCount = "SELECT COUNT(*) " +
            "FROM RETAILTRANSACTIONTABLE AS H " +
            sqlWhereBatchClause +
            "AND [TYPE] = @TRANSACTIONTYPE";

        private const string sqlBatchStartDateTime = "SELECT MIN(CREATEDDATE) " +
            "FROM RETAILTRANSACTIONTABLE AS H " +
            sqlWhereBatchClause;

        private const string sqlIncomeExpense = "SELECT INCOMEEXEPENSEACCOUNT, ACCOUNTTYPE, SUM(AMOUNT) AS AMOUNT " +
            "FROM RETAILTRANSACTIONINCOMEEXPENSETRANS AS L " +
            sqlHeaderJoinClause1 +
            sqlWhereBatchClause +
            "AND ENTRYSTATUS = @TRANSACTIONSTATUS " +
            "AND TRANSACTIONSTATUS = @TRANSACTIONSTATUS " +
            "AND [TYPE] = @TRANSACTIONTYPE " +
            "GROUP BY ACCOUNTTYPE, INCOMEEXEPENSEACCOUNT " +
            "ORDER BY INCOMEEXEPENSEACCOUNT";

        private const string sqlTenderLines = "SELECT TENDERTYPE, T.NAME, L.CURRENCY, COUNTINGREQUIRED, " +
            "SUM(AMOUNTTENDERED) AS AMOUNT, SUM(AMOUNTCUR) AS AMOUNTCUR " +
            "FROM {0} AS L " + // tableName from tenderLinesQueries Dictionary
            sqlHeaderJoinClause1 +
            sqlStoreTenderJoinClause +
            sqlWhereBatchClause +
            "AND TRANSACTIONSTATUS = @TRANSACTIONSTATUS " +
            "AND (COUNTINGREQUIRED = 1 OR TAKENTOBANK = 1 OR TAKENTOSAFE = 1) " +
            "AND [TYPE] = (@TRANSACTIONTYPE) " +
            "GROUP BY TENDERTYPE, T.NAME, L.CURRENCY, COUNTINGREQUIRED";

        private const string sqlLastTenderDeclarationLine = "SELECT L.CURRENCY, L.TRANSDATE, L.TRANSTIME, TENDERTYPE, T.NAME, " +
           "COUNTINGREQUIRED, AMOUNTTENDERED AS AMOUNT, AMOUNTCUR AS AMOUNTCUR " +
           "FROM {0} AS L " + // tableName from tenderLinesQueries Dictionary
           sqlLastTenderDeclarationClause +
           "ORDER BY L.TRANSACTIONID DESC";

        private const string sqlLastZeroAsZeroTenderDeclarationLine = "SELECT L.CURRENCY, L.TRANSDATE, L.TRANSTIME, TENDERTYPE, T.NAME, " +
            "COUNTINGREQUIRED, AMOUNTTENDERED AS AMOUNT, AMOUNTCUR AS AMOUNTCUR " +
            "FROM {0} AS L " + // tableName from tenderLinesQueries Dictionary
            sqlLastTenderDeclarationClause +
            "AND L.TRANSACTIONID IN" + 
            "(SELECT MAX(L2.TRANSACTIONID) " +
            "FROM {1} AS L2 " + // tableName from tenderLinesQueries Dictionary
            sqlLastTenderDeclarationClause2 + 
            ")";

        private const string sqlLastTenderDeclarationClause = sqlHeaderJoinClause1 +
            sqlStoreTenderJoinClause +
            sqlWhereBatchClause +
            "AND TRANSACTIONSTATUS = @TRANSACTIONSTATUS " +
            "AND (COUNTINGREQUIRED = 1 OR TAKENTOBANK = 1 OR TAKENTOSAFE = 1) " +
            "AND [TYPE] = (@TRANSACTIONTYPE) ";

        private const string sqlLastTenderDeclarationClause2 = sqlHeaderJoinClause2 +
            sqlStoreTenderJoinClause2 +
            sqlWhereBatchClause +
            "AND TRANSACTIONSTATUS = @TRANSACTIONSTATUS " +
            "AND (COUNTINGREQUIRED = 1 OR TAKENTOBANK = 1 OR TAKENTOSAFE = 1) " +
            "AND [TYPE] = (@TRANSACTIONTYPE) ";

        private const string sqlTenderDeclarationCalculationType = "SELECT TENDERDECLARATIONCALCULATION " +
            "FROM RETAILSTORETABLE WHERE STORENUMBER = @STORE";

        private const string sqlTenderZeroDeclarationType = "SELECT ZEROTENDERDECLARATION " +
            "FROM RETAILSTORETABLE WHERE STORENUMBER = @STORE";

        private static Dictionary<TypeOfTransaction, string> tenderLinesQueries
                = new Dictionary<TypeOfTransaction, string>()
                {
                    {TypeOfTransaction.StartingAmount, "RETAILTRANSACTIONPAYMENTTRANS" },
                    {TypeOfTransaction.FloatEntry, "RETAILTRANSACTIONPAYMENTTRANS" },
                    {TypeOfTransaction.RemoveTender, "RETAILTRANSACTIONPAYMENTTRANS" },
                    {TypeOfTransaction.SafeDrop, "RETAILTRANSACTIONSAFETENDERTRANS" },
                    {TypeOfTransaction.BankDrop, "RETAILTRANSACTIONBANKEDTENDERTRANS"},
                    {TypeOfTransaction.TenderDeclaration, "RETAILTRANSACTIONTENDERDECLARATIONTRANS" }
                };

        private const string sqlTenderCalculatedLines = "SELECT TENDERTYPE, T.NAME, L.CURRENCY, COUNTINGREQUIRED, " +
            "CHANGELINE, SUM(AMOUNTTENDERED) AS AMOUNT, SUM(AMOUNTCUR) AS AMOUNTCUR, COUNT(*) AS COUNT " +
            "FROM RETAILTRANSACTIONPAYMENTTRANS AS L " +
            sqlHeaderJoinClause1 +
            sqlStoreTenderJoinClause +
            sqlWhereBatchClause +
            "AND TRANSACTIONSTATUS = @TRANSACTIONSTATUS " +
            "AND [TYPE] IN (@TYPE1, @TYPE2, @TYPE3, @TYPE4, @TYPE5, @TYPE6) " +
            "GROUP BY TENDERTYPE, T.NAME, L.CURRENCY, CHANGELINE, COUNTINGREQUIRED";

        private const string sqlVerifyTransactionTable = "SELECT TRANSACTIONID " +
            "FROM RETAILTRANSACTIONTABLE AS H " +
            sqlWhereBatchClause +
            "AND TERMINAL = @TERMINALID ";

        #endregion

        #region Methods

        /// <summary>
        /// Calculate a batch using Transactions.
        /// </summary>
        /// <param name="batch"></param>
        public static void Calculate(this Batch batch)
        {
            DbConnection connection = ApplicationSettings.Database.LocalConnection;

            try
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (DbCommand dBCommand = new SqlCommand())
                {
                    dBCommand.Connection = connection;

                    CalculateHeader(dBCommand, batch);
                    CalculateIncomeExpenseAccounts(dBCommand, batch);
                    CalculateTender(dBCommand, batch);
                }
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        /// <summary>
        /// Verify if offline transactions has been uploaded to Store Db.
        /// </summary>
        /// <param name="posBatch"></param>
        /// <returns>True if all transactions has been uploaded, false otherwise.</returns>
        public static bool VerifyOfflineTransactions(this Batch batch)
        {
            bool result = true; // optimistic approach
            SqlConnection storeDbConnection = EOD.InternalApplication.Settings.Database.Connection; // This method is only called when we are online, so this will be store db.
            SqlConnection offlineConnection = EOD.InternalApplication.Settings.Database.OfflineConnection;

            // If offline db is not configured then no need to verify.
            if (offlineConnection != null)
            {
                List<string> offlineTransactions = ReadTransactions(batch, offlineConnection);

                if (offlineTransactions.Count > 0)
                {
                    List<string> storeDbTransactions = ReadTransactions(batch, storeDbConnection);

                    if (offlineTransactions.Except(storeDbTransactions).Count() > 0)
                    {
                        // We have some missing transactions in store db.
                        result = false;
                    }
                }
            }

            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Calculate Header
        /// </summary>
        /// <param name="dBCommand"></param>
        /// <param name="batch"></param>
        private static void CalculateHeader(DbCommand dBCommand, Batch batch)
        {
            dBCommand.Initialize(sqlBatchStartDateTime, batch);
            batch.StartDateTime = DBUtil.ToDateTime(dBCommand.ExecuteScalar());

            dBCommand.Initialize(sqlSalesTotal, batch);
            dBCommand.AddParameter("@TRANSACTIONSTATUS", TransactionStatus.Normal);
            dBCommand.AddParameter("@TRANSACTIONSTATUSFORCUSTOMERORDER", TransactionStatus.Posted);
            dBCommand.AddParameter("@TYPECUSTOMERORDER", TypeOfTransaction.CustomerOrder);
            dBCommand.AddParameter("@TAXINCLUSIVE", ApplicationSettings.Terminal.TaxIncludedInPrice);
            batch.SalesTotal = DBUtil.ToDecimal(dBCommand.ExecuteScalar());

            dBCommand.Initialize(sqlReturnsTotal, batch);
            dBCommand.AddParameter("@TRANSACTIONSTATUS", TransactionStatus.Normal);
            dBCommand.AddParameter("@TRANSACTIONSTATUSFORCUSTOMERORDER", TransactionStatus.Posted);
            dBCommand.AddParameter("@TYPECUSTOMERORDER", TypeOfTransaction.CustomerOrder);
            dBCommand.AddParameter("@TAXINCLUSIVE", ApplicationSettings.Terminal.TaxIncludedInPrice);
            batch.ReturnsTotal = DBUtil.ToDecimal(dBCommand.ExecuteScalar());

            dBCommand.Initialize(sqlTaxTotal, batch);
            dBCommand.AddParameter("@TRANSACTIONSTATUS", TransactionStatus.Normal);
            dBCommand.AddParameter("@TRANSACTIONSTATUSFORCUSTOMERORDER", TransactionStatus.Posted);
            dBCommand.AddParameter("@TYPECUSTOMERORDER", TypeOfTransaction.CustomerOrder);
            batch.TaxTotal = Decimal.Negate(DBUtil.ToDecimal(dBCommand.ExecuteScalar()));

            dBCommand.Initialize(sqlDiscountTotal, batch);
            dBCommand.AddParameter("@TRANSACTIONSTATUS", TransactionStatus.Normal);
            dBCommand.AddParameter("@TRANSACTIONSTATUSFORCUSTOMERORDER", TransactionStatus.Posted);
            dBCommand.AddParameter("@TYPECUSTOMERORDER", TypeOfTransaction.CustomerOrder);
            batch.DiscountTotal = DBUtil.ToDecimal(dBCommand.ExecuteScalar());

            dBCommand.Initialize(sqlPaidToAccountTotal, batch);
            dBCommand.AddParameter("@TRANSACTIONSTATUS", TransactionStatus.Normal);
            dBCommand.AddParameter("@TRANSACTIONTYPE", TypeOfTransaction.Payment);
            batch.PaidToAccountTotal = DBUtil.ToDecimal(dBCommand.ExecuteScalar());

            dBCommand.Initialize(sqlRoundingAmountTotal, batch);
            batch.RoundedAmountTotal = DBUtil.ToDecimal(dBCommand.ExecuteScalar());

            dBCommand.Initialize(sqlSalesCount, batch);
            dBCommand.AddParameter("@TRANSACTIONSTATUS", TransactionStatus.Normal);
            dBCommand.AddParameter("@TRANSACTIONSTATUSFORCUSTOMERORDER", TransactionStatus.Posted);
            dBCommand.AddParameter("@TYPECUSTOMERORDER", TypeOfTransaction.CustomerOrder);
            batch.SalesCount = DBUtil.ToInt32(dBCommand.ExecuteScalar());

            dBCommand.Initialize(sqlVoidsCount, batch);
            dBCommand.AddParameter("@TRANSACTIONSTATUS", TransactionStatus.Voided);
            batch.VoidsCount = DBUtil.ToInt32(dBCommand.ExecuteScalar());

            dBCommand.Initialize(sqlCustomersCount, batch);
            dBCommand.AddParameter("@TRANSACTIONSTATUS", TransactionStatus.Normal);
            dBCommand.AddParameter("@TRANSACTIONSTATUSFORCUSTOMERORDER", TransactionStatus.Posted);
            dBCommand.AddParameter("@TYPECUSTOMERORDER", TypeOfTransaction.CustomerOrder);
            batch.CustomersCount = DBUtil.ToInt32(dBCommand.ExecuteScalar());

            dBCommand.Initialize(sqlTransactionTypesCount, batch);
            dBCommand.AddParameter("@TRANSACTIONTYPE", TypeOfTransaction.OpenDrawer);
            batch.NoSaleCount = DBUtil.ToInt32(dBCommand.ExecuteScalar());

            dBCommand.Initialize(sqlTransactionTypesCount, batch);
            dBCommand.AddParameter("@TRANSACTIONTYPE", TypeOfTransaction.LogOn);
            batch.LogOnCount = DBUtil.ToInt32(dBCommand.ExecuteScalar());

            dBCommand.Initialize(sqlTransactonsCount, batch);
            batch.TransactionsCount = DBUtil.ToInt32(dBCommand.ExecuteScalar()) + 1;

            dBCommand.Initialize(sqlSuspendedTransactonCount);
            batch.SuspendedTransactionsCount = DBUtil.ToInt32(dBCommand.ExecuteScalar());

        }

        /// <summary>
        /// Income/Expense Accounts
        /// </summary>
        /// <param name="dBCommand"></param>
        /// <param name="batch"></param>
        private static void CalculateIncomeExpenseAccounts(DbCommand dBCommand, Batch batch)
        {
            dBCommand.Initialize(sqlIncomeExpense, batch);
            dBCommand.AddParameter("@TRANSACTIONSTATUS", TransactionStatus.Normal);
            dBCommand.AddParameter("@TRANSACTIONTYPE", TypeOfTransaction.IncomeExpense);
            using (DbDataReader reader = dBCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    BatchAccountLine batchAccountLine = new BatchAccountLine();

                    batchAccountLine.AccountNumber = (string)reader["INCOMEEXEPENSEACCOUNT"];
                    batchAccountLine.AccountType = DBUtil.ToEnum<IncomeExpenseAccountType>(reader["ACCOUNTTYPE"]);
                    batchAccountLine.Amount = DBUtil.ToDecimal(reader["AMOUNT"]);

                    if (batchAccountLine.AccountType == IncomeExpenseAccountType.Income)
                        batchAccountLine.Amount = Decimal.Negate(batchAccountLine.Amount);

                    batch.AccountLines.Add(batchAccountLine);
                }
            }
        }

        /// <summary>
        /// Tender Lines
        /// </summary>
        /// <param name="dBCommand"></param>
        /// <param name="batch"></param>
        private static void CalculateTender(DbCommand dBCommand, Batch batch)
        {
            dBCommand.Initialize(sqlTenderDeclarationCalculationType);
            dBCommand.AddParameter("@STORE", ApplicationSettings.Terminal.StoreId);
            tenderDeclCalculationType = DBUtil.ToEnum<TenderDeclarationCalculationType>(dBCommand.ExecuteScalar());

            dBCommand.Initialize(sqlTenderZeroDeclarationType);
            dBCommand.AddParameter("@STORE", ApplicationSettings.Terminal.StoreId);
            tenderZeroDeclarationType = DBUtil.ToEnum<TenderZeroDeclarationType>(dBCommand.ExecuteScalar());

            // Calculate Tender addition/removal
            foreach (KeyValuePair<TypeOfTransaction, string> query in tenderLinesQueries)
            {
                bool useLastTenderDeclaration = false;

                // Checking store level setting whether Tender Declaration Calculation Type is Last or Sum.
                if (query.Key.Equals(PosTransaction.TypeOfTransaction.TenderDeclaration) && tenderDeclCalculationType.Equals(TenderDeclarationCalculationType.Last))
                {
                    dBCommand.Initialize(string.Format(sqlLastTenderDeclarationLine, query.Value), batch);
                    useLastTenderDeclaration = true;
                    
                    if (tenderZeroDeclarationType.Equals(TenderZeroDeclarationType.ZeroAsZero))
                    {
                        dBCommand.Initialize(string.Format(sqlLastZeroAsZeroTenderDeclarationLine, query.Value, query.Value), batch);
                    }
                }
                else
                {
                    dBCommand.Initialize(string.Format(sqlTenderLines, query.Value), batch);
                }
                dBCommand.AddParameter("@TRANSACTIONSTATUS", TransactionStatus.Normal);
                dBCommand.AddParameter("@TRANSACTIONTYPE", (int)query.Key);
                using (DbDataReader reader = dBCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!(useLastTenderDeclaration && batch.TenderLines.ExistsTenderDeclarationTrans(reader)))
                        {
                            BatchTenderLine batchTenderLine = batch.TenderLines.FindOrCreate(reader);
                            decimal amount = DBUtil.ToDecimal(reader["AMOUNT"]);
                            decimal amountCur = DBUtil.ToDecimal(reader["AMOUNTCUR"]);  

                            batchTenderLine.CountingRequired = DBUtil.ToBool(reader["COUNTINGREQUIRED"]);

                            switch (query.Key)
                            {
                                case TypeOfTransaction.StartingAmount:
                                    batchTenderLine.StartingAmount = amount;
                                    batchTenderLine.StartingAmountCur = amountCur;
                                    break;

                                case TypeOfTransaction.FloatEntry:
                                    batchTenderLine.FloatEntryAmount = amount;
                                    batchTenderLine.FloatEntryAmountCur = amountCur;
                                    break;

                                case TypeOfTransaction.RemoveTender:
                                    batchTenderLine.RemoveTenderAmount = decimal.Negate(amount);
                                    batchTenderLine.RemoveTenderAmountCur = decimal.Negate(amountCur);
                                    break;

                                case TypeOfTransaction.SafeDrop:
                                    batchTenderLine.SafeDropAmount = amount;
                                    batchTenderLine.SafeDropAmountCur = amountCur;
                                    break;

                                case TypeOfTransaction.BankDrop:
                                    batchTenderLine.BankDropAmount = amount;
                                    batchTenderLine.BankDropAmountCur = amountCur;
                                    break;

                                case TypeOfTransaction.TenderDeclaration:
                                    batchTenderLine.DeclareTenderAmount = amount;
                                    batchTenderLine.DeclareTenderAmountCur = amountCur;
                                    break;

                                default:
                                    String message = "Unsupported transaction type";
                                    NetTracer.Error(message);
                                    throw new NotSupportedException(message);
                            }
                        }
                    }
                }
            }

            // Calcualte tendered and change.
            dBCommand.Initialize(sqlTenderCalculatedLines, batch);
            dBCommand.AddParameter("@TRANSACTIONSTATUS", TransactionStatus.Normal);
            dBCommand.AddParameter("@TYPE1", TypeOfTransaction.Sales);
            dBCommand.AddParameter("@TYPE2", TypeOfTransaction.Payment);
            dBCommand.AddParameter("@TYPE3", TypeOfTransaction.SalesInvoice);
            dBCommand.AddParameter("@TYPE4", TypeOfTransaction.SalesOrder);
            dBCommand.AddParameter("@TYPE5", TypeOfTransaction.CustomerOrder);
            dBCommand.AddParameter("@TYPE6", TypeOfTransaction.IncomeExpense);
            using (DbDataReader reader = dBCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    BatchTenderLine batchTenderLine = batch.TenderLines.FindOrCreate(reader);
                    decimal amount = DBUtil.ToDecimal(reader["AMOUNT"]);
                    decimal amountCur = DBUtil.ToDecimal(reader["AMOUNTCUR"]);

                    if (DBUtil.ToBool(reader["CHANGELINE"]))
                    {
                        batchTenderLine.ChangeAmount = decimal.Negate(amount);
                        batchTenderLine.ChangeAmountCur = decimal.Negate(amountCur);
                    }
                    else
                    {
                        batchTenderLine.TenderedAmount = amount;
                        batchTenderLine.TenderedAmountCur = amountCur;
                        batchTenderLine.Count = DBUtil.ToInt32(reader["COUNT"]);
                    }

                    batchTenderLine.CountingRequired = DBUtil.ToBool(reader["COUNTINGREQUIRED"]);
                }
            }
        }

        /// <summary>
        /// Initialize dBCommand with query and required parameters.
        /// </summary>
        /// <remarks>CA2100 The queries are already parametrized. No sql injection threat.</remarks>
        /// <param name="dBCommand"></param>
        /// <param name="query"></param>
        /// <param name="posBatch"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "The queries are already parametrized. No sql injection threat.")]
        private static void Initialize(this DbCommand dBCommand, string query, Batch posBatch = null)
        {
            dBCommand.CommandText = query;
            dBCommand.Parameters.Clear();

            if (posBatch != null)
            {
                dBCommand.Parameters.Add(new SqlParameter("@STOREID", posBatch.StoreId));
                dBCommand.Parameters.Add(new SqlParameter("@BATCHTERMINALID", posBatch.TerminalId));
                dBCommand.Parameters.Add(new SqlParameter("@BATCHID", posBatch.BatchId));
            }
            else
            {
                dBCommand.Parameters.Add(new SqlParameter("@STOREID", ApplicationSettings.Terminal.StoreId));
                dBCommand.Parameters.Add(new SqlParameter("@TERMINALID", ApplicationSettings.Terminal.TerminalId));
            }

            dBCommand.Parameters.Add(new SqlParameter("@DATAAREAID", ApplicationSettings.Database.DATAAREAID));
        }

        /// <summary>
        /// Add parameter to dbCommand
        /// </summary>
        /// <param name="dBCommand"></param>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        private static void AddParameter(this DbCommand dBCommand, string parameterName, object value)
        {
            dBCommand.Parameters.Add(new SqlParameter(parameterName, value));
        }

        /// <summary>
        /// Find or create a batch tender line.
        /// </summary>
        /// <param name="batchTenderLines"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static BatchTenderLine FindOrCreate(this Collection<BatchTenderLine> batchTenderLines, DbDataReader reader)
        {
            string tenderTypeId = (string)reader["TENDERTYPE"];
            string currency = (string)reader["CURRENCY"];
            string name = (string)reader["NAME"];
            BatchTenderLine batchTenderLine = batchTenderLines.FirstOrDefault(p => p.TenderTypeId == tenderTypeId && p.Currency == currency);

            if (batchTenderLine == null)
            {
                batchTenderLine = new BatchTenderLine();
                batchTenderLine.TenderTypeId = tenderTypeId;
                batchTenderLine.CardTypeId = string.Empty;
                batchTenderLine.Currency = currency;
                batchTenderLine.TenderName = name;
                batchTenderLines.Add(batchTenderLine);
            }

            return batchTenderLine;
        }

        /// <summary>
        /// Check whether the last tender declaration line exists in the Collection
        /// </summary>
        /// <param name="batchTenderLines"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static bool ExistsTenderDeclarationTrans(this Collection<BatchTenderLine> batchTenderLines, DbDataReader reader)
        {
            string tenderTypeId = (string)reader["TENDERTYPE"];
            string currency = (string)reader["CURRENCY"];
            BatchTenderLine batchTenderLine = batchTenderLines.FirstOrDefault(p => p.TenderTypeId == tenderTypeId && p.Currency == currency);

            return batchTenderLine != null && (batchTenderLine.DeclareTenderAmount != 0 || batchTenderLine.DeclareTenderAmountCur != 0);
        }

        /// <summary>
        /// Read transaction for a given batch.
        /// </summary>
        /// <param name="batch">Batch object</param>
        /// <param name="connection">Connection to use</param>
        /// <returns>List of transaction ids.</returns>
        private static List<string> ReadTransactions(Batch batch, SqlConnection connection)
        {
            List<string> transactionIds = new List<string>();

            try
            {
                using (SqlCommand command = new SqlCommand(sqlVerifyTransactionTable, connection))
                {
                    command.Parameters.Add(new SqlParameter("@BATCHTERMINALID", batch.TerminalId));
                    command.Parameters.Add(new SqlParameter("@BATCHID", batch.BatchId));
                    command.Parameters.Add(new SqlParameter("@STOREID", ApplicationSettings.Terminal.StoreId));
                    command.Parameters.Add(new SqlParameter("@TERMINALID", ApplicationSettings.Terminal.TerminalId));
                    command.Parameters.Add(new SqlParameter("@DATAAREAID", EOD.InternalApplication.Settings.Database.DataAreaID));

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            transactionIds.Add(DBUtil.ToStr(reader["TRANSACTIONID"]));
                        }
                    }
                }
            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }

            return transactionIds;
        }

        #endregion

    }
}
