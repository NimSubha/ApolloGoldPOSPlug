/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.ComponentModel.Composition;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using LSRetailPosis.DataAccess.DataUtil;
using LSRetailPosis.Transaction;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;

namespace Microsoft.Dynamics.Retail.Pos.RFID
{
    [Export(typeof(IRFID))]
    public class RFID : IRFID
    {
        [Import]
        public IApplication Application { get; set; }

        #region IRFID Members

        /// <summary>
        /// Returns all RFID's that have not been used as ScanInfo[].
        /// </summary>
        /// <returns></returns>
        public IScanInfo[] GetUnProcessedRFIDs()
        {
            try
            {
                //Get all the RFIDs currently in the PosisRfidTable that have not been used
                string sqlQry = " SELECT R.RFID, ISNULL(S.ITEMID, '') AS ITEMID " +
                                " FROM POSISRFIDTABLE R " +
                                " LEFT JOIN INVENTSERIAL S ON S.RFIDTAGID = R.RFID " +
                                " WHERE TRANSACTIONID IS NULL ";
                DataTable RFIDtable = new DBUtil(Application.Settings.Database.Connection).GetTable(sqlQry);

                //Create the array of RfidInformation
                IScanInfo[] scanInfo = new IScanInfo[RFIDtable.Rows.Count];

                int i = 0;
                foreach (DataRow row in RFIDtable.Rows)
                {
                    scanInfo[i] = this.Application.BusinessLogic.Utility.CreateScanInfo();
                    scanInfo[i].ScanDataLabel = row["ITEMID"].ToString();
                    scanInfo[i].RFIDTag = row["RFID"].ToString();
                    scanInfo[i++].EntryType = BarcodeEntryType.MultiScanned;
                }

                return scanInfo;
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException("RFID.ProcessRFID", x);
                throw;
            }
        }

        /// <summary>
        /// Marks only processed RFID's.
        /// </summary>
        /// <param name="transaction"></param>
        public void MarkProcessedRFIDs(IRetailTransaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");

            string updList = "";

            //Go through the RFIDs scanned and add them to the RFIDInfo class
            foreach (ISaleLineItem item in ((RetailTransaction)transaction).SaleItems)
            {
                if (!string.IsNullOrEmpty(item.RFIDTagId))
                {
                    //Create an update list that will be used to update all the RFID's read in the previous
                    //sql query. The list is created in case more RFID's might have been read since the query was executed.
                    if (updList.Length != 0) { updList += ", "; }
                    updList += "'" + item.RFIDTagId + "'";
                }
            }

            if (updList.Length != 0)
            {
                //Update the read RFIDs with the current transaction id.
                string sqlUpd = " UPDATE POSISRFIDTABLE " +
                                " SET TRANSACTIONID = '" + transaction.TransactionId + "' " +
                                " WHERE RFID IN (" + updList + ") ";
                DBUtil dbUtil = new DBUtil(Application.Settings.Database.Connection);
                dbUtil.Execute(sqlUpd);
            }
        }

        /// <summary>
        /// Displays transaction information.
        /// </summary>
        /// <param name="posTransaction"></param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "posTransaction")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Grandfather")]
        public void SendTransactionInfo(IPosTransaction posTransaction)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }
        /// <summary>
        /// Deletes the RFID's from database.
        /// </summary>
        public void ConcludeRFIDs()
        {
            DBUtil dbUtil = new DBUtil(Application.Settings.Database.Connection);
            dbUtil.Execute("DELETE FROM POSISRFIDTABLE");
        }

        #endregion
    }
}
