//Microsoft Dynamics AX for Retail POS Plug-ins
//The following project is provided as SAMPLE code. Because this software is "as is," we may not provide support services for it.

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using LSRetailPosis.DataAccess.DataUtil;
using LSRetailPosis.DevUtilities;
using System.Diagnostics;

namespace Microsoft.Dynamics.Retail.Pos.CreateDatabaseService
{

    internal class ImportInitialData
    {

        #region Fields

        private DBUtil dbUtil;
        private Action<string> importStatusCallback;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// Import all available data into database.
        /// </summary>
        /// <param name="dbUtil"></param>
        /// <param name="dataAreaID"></param>
        /// <param name="statusCallback"></param>
        internal ImportInitialData(DBUtil dbUtil, Action<string> statusCallback)
        {
            // Get all text through the Translation function in the ApplicationLocalizer
            // TextID's for CorporateCard are reserved at 50600 - 50699
            //
            // These TextID's are in every class in the CreateDatabase project

            this.dbUtil = dbUtil;
            this.importStatusCallback = statusCallback;
        }

        /// <summary>
        /// Import all available tables into database.
        /// </summary>
        public void ImportData()
        {
            Debug.Assert(importStatusCallback != null);

            ImportInfoPosisInfo();
            ImportIntoPosisErrors();
        }

        /// <summary>
        /// Get table data from resource.
        /// </summary>
        /// <remarks>Caller is responsible for disposing returned object</remarks>
        /// <param name="tableName">Table resource name.</param>
        /// <returns>Caller owned datatable.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller is responsible for disposing returned object")]
        public static DataTable GetData(string tableName)
        {
            DataTable data = new DataTable();
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (StreamReader streamReader = new StreamReader(assembly.GetManifestResourceStream("CreateDatabase." + tableName + ".xml")))
            {
                data.ReadXml(streamReader);
            }

            return data;
        }

        #endregion

        #region Private Methods

        private void RaiseTableImported(string tableName)
        {
            if (importStatusCallback != null && tableName != null)
                importStatusCallback(tableName);
        }

        private void ImportIntoPosisErrors()
        {
            string tableName = "RETAILERRORS";

            using (DataTable impTable = GetData("DemoData." + tableName))
            {
                foreach (DataRow row in impTable.Select())
                {
                    SqlInsert sqlInsert = new SqlInsert(tableName);
                    if (row["ERRORID"] != DBNull.Value)
                        sqlInsert.Add("ERRORID", Convert.ToInt32(row["ERRORID"]));
                    if (row["ERRORMESSAGEID"] != DBNull.Value)
                        sqlInsert.Add("ERRORMESSAGEID", Convert.ToInt32(row["ERRORMESSAGEID"]));
                    if (row["DESCRIPTION"] != DBNull.Value)
                        sqlInsert.Add("DESCRIPTION", Convert.ToString(row["DESCRIPTION"]));
                    if (row["CODEUNIT"] != DBNull.Value)
                        sqlInsert.Add("CODEUNIT", Convert.ToString(row["CODEUNIT"]));
                    if (row["ACTIVE"] != DBNull.Value)
                        sqlInsert.Add("ACTIVE", Convert.ToInt32(row["ACTIVE"]));
                    if (row["FIRSTINVERSION"] != DBNull.Value)
                        sqlInsert.Add("FIRSTINVERSION", Convert.ToString(row["FIRSTINVERSION"]));
                    if (row["DATECREATED"] != DBNull.Value)
                        sqlInsert.Add("DATECREATED", Convert.ToDateTime(row["DATECREATED"]));

                    dbUtil.Execute(sqlInsert);
                }
            }
            RaiseTableImported(tableName);
        }

        public void ImportInfoPosisInfo()
        {
            string tableName = "POSISINFO";

            using (DataTable impTable = GetData("DemoData." + tableName))
            {
                foreach (DataRow row in impTable.Select())
                {
                    SqlInsert sqlInsert = new SqlInsert(tableName);
                    if (row["ID"] != DBNull.Value)
                        sqlInsert.Add("ID", Convert.ToString(row["ID"]));
                    if (row["TEXT"] != DBNull.Value)
                        sqlInsert.Add("TEXT", Convert.ToString(row["TEXT"]));

                    dbUtil.Execute(sqlInsert);
                }
            }
            RaiseTableImported(tableName);
        }

        #endregion

    }
}