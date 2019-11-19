/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Data.SqlClient;
using LSRetailPosis.DataAccess;
using LSRetailPosis.DataAccess.DataUtil;
using LSRetailPosis.Transaction;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;

namespace Microsoft.Dynamics.Retail.FiscalPrinter.RussianFiscalPrinter
{
    /// <summary>
    /// Class that provides access to Fiscal information on the database.
    /// </summary>
    public sealed class TransactionFiscalData : DataLayer
    {
        private const string RetailTransactionTable = "RetailTransactionTable_RU";

        /// <summary>
        /// TransactionFiscalData constructor.
        /// </summary>
        /// <param name="sqlCon">SqlConnetion</param>
        /// <param name="dataArea">DataArea</param>
        public TransactionFiscalData(SqlConnection sqlCon, string dataArea) :
            base(sqlCon, dataArea)
        {
        }

        /// <summary>
        /// Saves the POSTransaction fiscal data.
        /// </summary>
        /// <param name="transaction">The POS Transaction.</param>
        /// <param name="fiscalMemoryData">The Fiscal memory data.</param>
        public void Save(PosTransaction transaction, FiscalMemoryData fiscalMemoryData)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }

            if (fiscalMemoryData == null)
            {
                throw new ArgumentNullException("fiscalMemoryData");
            }

            var insert = new SqlInsert(RetailTransactionTable);

            insert.Add("TRANSACTIONID", transaction.TransactionId);
            insert.Add("STORE", transaction.StoreId);
            insert.Add("TERMINAL", transaction.TerminalId);
            insert.Add("DATAAREAID", dataAreaId);

            var retailTransaction = transaction as IRetailTransaction;

            if (retailTransaction != null)
            {
                insert.Add("CHANNEL", retailTransaction.ChannelId);
            }

            if (!string.IsNullOrEmpty(fiscalMemoryData.FiscalPrinterSerialNumber))
                insert.Add("FISCALPRINTERSERIALNUMBER", fiscalMemoryData.FiscalPrinterSerialNumber);
            if (!string.IsNullOrEmpty(fiscalMemoryData.FiscalPrinterShiftId))
                insert.Add("FISCALPRINTERSHIFTID", fiscalMemoryData.FiscalPrinterShiftId);
            if (!string.IsNullOrEmpty(fiscalMemoryData.EKLZSerialNumber))
                insert.Add("EKLZSERIALNUMBER", fiscalMemoryData.EKLZSerialNumber);

            if (transaction.TransactionType != PosTransaction.TypeOfTransaction.LogOn)
            {
                if (!string.IsNullOrEmpty(fiscalMemoryData.KPKNumber))
                    insert.Add("KPKNUMBER", fiscalMemoryData.KPKNumber);
                if (!string.IsNullOrEmpty(fiscalMemoryData.FiscalDocumentSerialNumber))
                    insert.Add("FISCALDOCUMENTSERIALNUMBER", fiscalMemoryData.FiscalDocumentSerialNumber);
            }

            dbUtil.Execute(insert);
        }

        /// <summary>
        /// Gets the Retail transaction fiscal shift ID.
        /// </summary>
        /// <param name="transaction">The Retail Transaction.</param>
        /// <returns>Shift ID</returns>
        public string GetFiscalShiftId(IRetailTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }

            string shiftId = null;

            var select = new SqlSelect(RetailTransactionTable);
            select.Select("FISCALPRINTERSHIFTID");
            select.Where("TRANSACTIONID", transaction.TransactionId, true);
            select.Where("STORE", transaction.StoreId, true);
            select.Where("TERMINAL", transaction.TerminalId, true);
            select.Where("CHANNEL", transaction.ChannelId, true);

            using (SqlDataReader reader = dbUtil.GetReader(select))
            {
                reader.Read();
                if (reader.HasRows)
                {
                    shiftId = DBUtil.ToStr(reader["FISCALPRINTERSHIFTID"]);
                }
            }

            return shiftId;
        }
    }
}
