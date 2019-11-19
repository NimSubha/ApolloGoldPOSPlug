//Microsoft Dynamics AX for Retail POS Plug-ins 
//The following project is provided as SAMPLE code. Because this software is "as is," we may not provide support services for it.

using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using LSRetailPosis;
using LSRetailPosis.DataAccess.DataUtil;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;

namespace Microsoft.Dynamics.Retail.Pos.CreateDatabaseService
{
    public class EmbeddedInstall : ICreateDatabase
    {
        // Get all text through the Translation function in the ApplicationLocalizer
        // TextID's for EmbeddedInstall are reserved at 50600 - 50699
        //
        // These TextID's are in every class in the CreateDatabase project

        #region Variables

        private ProgressForm progressForm;

        private const int DatabaseCreationTimeOut = 300; // 5 Minutes
        private const string MasterCatalog = "master";
        private const string getVersionSql = "SELECT TEXT FROM POSISINFO WHERE ID='DBVERSION'";
        private const string setVersionSql = "UPDATE POSISINFO SET TEXT = '{0}' WHERE ID='DBVERSION'";
        private const string dbExecuteRole = "db_executor";

        private static readonly string[] dbExceuteRolePermissions = new string[] { "EXECUTE" };
        private static readonly string[] defaultDBRoles = new string[] { "db_datareader", "db_datawriter", dbExecuteRole };

        public IApplication Application { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="storeID"></param>
        /// <param name="terminalID"></param>
        /// <param name="dataAreaID"></param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "storeID", Justification="Public API")]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "terminalID", Justification = "Public API")]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "dataAreaID", Justification = "Public API")]
        public EmbeddedInstall(string storeID, string terminalID, string dataAreaID)
        {
        }

        #endregion

        #region ICreateDatabase

        /// <summary>
        /// Check for database creation/upgradation.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="databasePhysicalFilePath"></param>
        /// <param name="includeDemoData">Flag to singal if demo data is needed</param>
        /// <param name="userGroupName">The windows user in the domain\user to configure access to the db.</param>
        public void CheckDatabase(string connectionString, string databasePhysicalFilePath, bool includeDemoData, string userGroupName)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new ArgumentNullException("connectionString");
                }

                if (string.IsNullOrWhiteSpace(databasePhysicalFilePath))
                {
                    throw new ArgumentNullException("databasePhysicalFilePath");
                }

                bool isDBUserAccessConfigured = !string.IsNullOrWhiteSpace(userGroupName);

                SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
                string databaseName = connectionStringBuilder.InitialCatalog;
                connectionStringBuilder.InitialCatalog = MasterCatalog;
                string masterConnectionString = connectionStringBuilder.ConnectionString;

                LogMessage("Master connection string: " + masterConnectionString);
                LogMessage("database name: " + databaseName);

                using (progressForm = new ProgressForm())
                {
                    using (DbConnection dBConnection = new SqlConnection(masterConnectionString))
                    {
                        using (DbCommand dBCommand = new SqlCommand())
                        {
                            dBConnection.Open();
                            dBCommand.Connection = dBConnection;

                            // Cannot create a parameterize query otherwise "Stored procedure 'sys.sp_dbcmptlevel' can only be executed at the ad hoc leve" exception will be thrown within CreateDatabase() method.
                            string queryString = string.Format(CultureInfo.CurrentCulture,
                                    "SELECT Count(*) FROM master..sysdatabases WHERE name = '{0}'", databaseName);
                            object result = dBCommand.ExecuteScalar(queryString);

                            if ((result == null) || (((int)result) == 0))
                            {
                                LogMessage("Database '{0}' is not found.", databaseName);

                                dBCommand.CommandTimeout = DatabaseCreationTimeOut;
                                CreateDatabase(databaseName, databasePhysicalFilePath, dBCommand);
                            }
                        }
                    }

                    // create the windows user group which will have access to the db
                    string qualifiedGroupName = null;
                    if (isDBUserAccessConfigured)
                    {
                        qualifiedGroupName = UserUtility.CreateUserGroup(userGroupName, Resources.DatabaseAccessUserGroupDescription);
                    }

                    using (DbConnection dBConnection = new SqlConnection(connectionString))
                    {
                        using (DbCommand dBCommand = new SqlCommand())
                        {
                            // it seems that sometime, even if we created the database above we will fail to open a connection to it.
                            // so we try first to see if we can open the connection.
                            dBConnection.TryOpen();
                            dBCommand.Connection = dBConnection;

                            // configure the db for user access
                            if (isDBUserAccessConfigured && !string.IsNullOrWhiteSpace(qualifiedGroupName))
                            {
								try
								{
									EmbeddedInstall.ConfigureDatabaseAccessForUser(dBCommand, qualifiedGroupName);
								}
								catch (Exception ex)
								{
									string message = string.Format("CheckDatabase failed while configuring database access:{0}", ex.ToString());
									LogMessage(message);
								}
                            }

                            // taking this out of the transaction as it will close the connection (as a result of failure) if the schema is not already created.
                            // this in turn will close the transaction and the last commit will fail.
                            string currentDbVersion = dBCommand.TryExecuteScalar(getVersionSql) as string;

                            using (DbTransaction dBTransaction = dBCommand.BeginTransaction(dBConnection))
                            {
                                // Create schema if doesn't exit.
                                if (currentDbVersion == null)
                                {
                                    SqlCommand sqlCommand = (SqlCommand)dBCommand;
                                    CreateSchema(dBCommand, new DBUtil(sqlCommand.Connection, sqlCommand.Transaction), includeDemoData);
                                }

                                UpgradeSchema(dBCommand);
                                dBTransaction.Commit();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("CheckDatabase failed:{0}", ex.ToString());
                LogMessage(message);
                throw new LSRetailPosis.PosStartupException(message, ex);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Create a new database
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="databasePhysicalFilePath"></param>
        /// <param name="masterConnectionString"></param>
        /// <exception cref="">FileNotFoundException</exception>
        private void CreateDatabase(string databaseName, string databasePhysicalFilePath, DbCommand dBCommand)
        {
            if (!Directory.Exists(databasePhysicalFilePath))
                throw new FileNotFoundException("Directory " + databasePhysicalFilePath + " does not exist.");

            DisplayMessage(50602, databaseName, dBCommand.Connection.DataSource); // Creating database

            string dbFileName = Path.Combine(databasePhysicalFilePath, databaseName + ".mdf");
            string dbLogName = Path.Combine(databasePhysicalFilePath, databaseName + "_log.mdf");

            #region Database creation

            LogMessage("Database filename: " + dbFileName);
            LogMessage("Database log filename: " + dbLogName);

            string sql = string.Format("CREATE DATABASE [{0}] ON PRIMARY ( NAME = N'{0}', FILENAME = N'{1}', SIZE = 1GB, MAXSIZE = UNLIMITED, FILEGROWTH = 30% ) " +
                    "LOG ON ( NAME = N'{0}_log', FILENAME = N'{2}', SIZE = 256MB, MAXSIZE = UNLIMITED, FILEGROWTH = 10% )", databaseName, dbFileName, dbLogName);

            dBCommand.ExecuteNonQuery(sql);
            LogMessage("Database created successfully.");

            dBCommand.ExecuteNonQuery(string.Format("dbo.sp_dbcmptlevel @dbname=N'{0}', @new_cmptlevel=100", databaseName));
            LogMessage("Compatibility level set to 100.");

            #endregion

            #region Database properties

            //Further reading - http://msdn2.microsoft.com/en-us/library/ms174269.aspx
            //The default value is NOT NULL in CREATE TABLE and ALTER TABLE statements
            dBCommand.ExecuteNonQuery("ALTER DATABASE [" + databaseName + "] SET ANSI_NULL_DEFAULT OFF");
            //Comparisons of non-UNICODE values to a null value evaluate to TRUE if both values are NULL.
            dBCommand.ExecuteNonQuery("ALTER DATABASE [" + databaseName + "] SET ANSI_NULLS OFF");
            //Trailing blanks for varchar or nvarchar and zeros for varbinary are trimmed.
            dBCommand.ExecuteNonQuery("ALTER DATABASE [" + databaseName + "] SET ANSI_PADDING OFF");
            //No warnings are raised and null values are returned when conditions such as divide-by-zero occur. 
            dBCommand.ExecuteNonQuery("ALTER DATABASE [" + databaseName + "] SET ANSI_WARNINGS OFF");
            //A warning message is displayed when one of these errors occurs, but the query, batch, or transaction continues to process as if no error occurred. 
            dBCommand.ExecuteNonQuery("ALTER DATABASE [" + databaseName + "] SET ARITHABORT OFF");
            //The database remains open after the last user exits.
            dBCommand.ExecuteNonQuery("ALTER DATABASE [" + databaseName + "] SET AUTO_CLOSE OFF");
            //Any missing statistics required by a query for optimization are automatically built during query optimization. 
            dBCommand.ExecuteNonQuery("ALTER DATABASE [" + databaseName + "] SET AUTO_CREATE_STATISTICS ON");
            //The database files are not automatically shrunk during periodic checks for unused space. 
            dBCommand.ExecuteNonQuery("ALTER DATABASE [" + databaseName + "] SET AUTO_SHRINK OFF");
            //Any out-of-date statistics required by a query for optimization are automatically updated during query optimization. 
            dBCommand.ExecuteNonQuery("ALTER DATABASE [" + databaseName + "] SET AUTO_UPDATE_STATISTICS ON");
            //Cursors remain open when a transaction is committed; rolling back a transaction closes any cursors except those defined as INSENSITIVE or STATIC. 
            dBCommand.ExecuteNonQuery("ALTER DATABASE [" + databaseName + "] SET CURSOR_CLOSE_ON_COMMIT OFF");
            //Controls whether cursor scope uses LOCAL or GLOBAL. 
            dBCommand.ExecuteNonQuery("ALTER DATABASE [" + databaseName + "] SET CURSOR_DEFAULT GLOBAL");
            //The null value is treated as an empty character string.
            dBCommand.ExecuteNonQuery("ALTER DATABASE [" + databaseName + "] SET CONCAT_NULL_YIELDS_NULL OFF");
            //Losses of precision do not generate error messages and the result is rounded to the precision of the column or variable storing the result. 
            dBCommand.ExecuteNonQuery("ALTER DATABASE [" + databaseName + "] SET NUMERIC_ROUNDABORT OFF");
            //Identifiers cannot be in quotation marks and must follow all Transact-SQL rules for identifiers. Literals can be delimited by either single or double quotation marks.
            dBCommand.ExecuteNonQuery("ALTER DATABASE [" + databaseName + "] SET QUOTED_IDENTIFIER OFF");
            //Only direct recursive firing of AFTER triggers is not allowed. To also disable indirect recursion of AFTER triggers, set the nested triggers server option to 0 by using sp_configure. 
            dBCommand.ExecuteNonQuery("ALTER DATABASE [" + databaseName + "] SET RECURSIVE_TRIGGERS OFF");
            //Specifies that Service Broker is disabled for the specified database. The is_broker_enabled flag is set to false in the sys.databases catalog view and message delivery is stopped.
            dBCommand.ExecuteNonQuery("ALTER DATABASE [" + databaseName + "] SET DISABLE_BROKER");
            //Queries that initiate an automatic update of out-of-date statistics wait until the updated statistics can be used in the query optimization plan. 
            dBCommand.ExecuteNonQuery("ALTER DATABASE [" + databaseName + "] SET AUTO_UPDATE_STATISTICS_ASYNC OFF");
            //Correlation statistics are not maintained.
            dBCommand.ExecuteNonQuery("ALTER DATABASE [" + databaseName + "] SET DATE_CORRELATION_OPTIMIZATION OFF");
            //Database modules in an impersonation context cannot access resources outside the database. 
            dBCommand.ExecuteNonQuery("ALTER DATABASE [" + databaseName + "] SET TRUSTWORTHY OFF");
            //Transactions can specify the SNAPSHOT transaction isolation level.
            dBCommand.ExecuteNonQuery("ALTER DATABASE [" + databaseName + "] SET ALLOW_SNAPSHOT_ISOLATION ON");
            //Queries are parameterized based on the default behavior of the database
            dBCommand.ExecuteNonQuery("ALTER DATABASE [" + databaseName + "] SET PARAMETERIZATION SIMPLE");
            //The database is available for read and write operations. 
            dBCommand.ExecuteNonQuery("ALTER DATABASE [" + databaseName + "] SET READ_WRITE");
            //A simple backup strategy that uses minimal log space is provided. Log space can be automatically reused when it is no longer required for server failure recovery
            dBCommand.ExecuteNonQuery("ALTER DATABASE [" + databaseName + "] SET RECOVERY SIMPLE");
            //All users that have the appropriate permissions to connect to the database are allowed.
            dBCommand.ExecuteNonQuery("ALTER DATABASE [" + databaseName + "] SET MULTI_USER");
            //Calculates a checksum over the contents of the whole page and stores the value in the page header when a page is written to disk
            dBCommand.ExecuteNonQuery("ALTER DATABASE [" + databaseName + "] SET PAGE_VERIFY CHECKSUM");
            //Database cannot participate in cross-database ownership chaining.
            dBCommand.ExecuteNonQuery("ALTER DATABASE [" + databaseName + "] SET DB_CHAINING OFF");

            LogMessage("AlterDatabase completed.");

            #endregion
        }

        /// <summary>
        /// Adds the specified user to the default roles for the db.
        /// </summary>
        /// <param name="dbConnection">The connection.</param>
        /// <param name="dBCommand">The dbcommand.</param>
        /// <param name="userName">The username.</param>
        private static void ConfigureDatabaseAccessForUser(DbCommand dBCommand, string userName)
        {
            Utilites.CreateAndGrantPermissionsToSqlRole(dBCommand, dbExecuteRole, dbExceuteRolePermissions);
            Utilites.CreateSqlLoginForWindowsUser(dBCommand, userName);
            Utilites.CreateSqlDBUserForSqlLogin(dBCommand, userName, userName); // note that the db username and the login name are the same
            Utilites.AddSqlDBUserToDBRoles(dBCommand, userName, defaultDBRoles);
        }

        /// <summary>
        /// Create Retail POS schema
        /// </summary>
        /// <param name="dBCommand"></param>
        /// <param name="dBUtil"></param>
        /// <param name="includeDemoData">Flag to signal if demo data is needed</param>
        private void CreateSchema(DbCommand dBCommand, DBUtil dBUtil, bool includeDemoData)
        {
            DisplayMessage(50604); //Running script

            dBCommand.ExecuteScript("DatabaseScript.txt");

            DisplayMessage(50605); //Tables successfully created.

            if (includeDemoData)
            {
                // Import demo data
                DisplayMessage(50606);  // Loading data into tables

                ImportInitialData importInitialData = new ImportInitialData(dBUtil,
                        (tableName) => DisplayMessage(50607, tableName));

                importInitialData.ImportData();
                DisplayMessage(50608);  // Data successfully loaded
            }

            string currentDbVersion = dBCommand.ExecuteScalar(getVersionSql) as string;

            // it seems that the version is set by the import data. 
            // If the import data is set to false, no version will be set and the script to create schema will be run again. 
            // This will fail if the tables already exists and the version was not set.
            if (currentDbVersion == null)
            {
                ImportInitialData importInitialData = new ImportInitialData(dBUtil,
                        (tableName) => DisplayMessage(50607, tableName));

                importInitialData.ImportInfoPosisInfo();
            }
        }

        private static Version GetUpgradeScriptVersion(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
                return new Version();
            return  new Version(version);
        }

        /// <summary>
        /// Upgrade the schema to latest one.
        /// </summary>
        /// <param name="dBCommand"></param>
        private void UpgradeSchema(DbCommand dBCommand)
        {
            if (dBCommand.Connection.State != ConnectionState.Open)
            {
                dBCommand.Connection.Open();
            }

            var currentDbVersion = GetUpgradeScriptVersion(dBCommand.ExecuteScalar(getVersionSql) as string);

            // Query the table for every upgrade greater than the current version
            using (DataTable upgradeTable = ImportInitialData.GetData("Upgrades.POSISUPGRADES"))
            {

                var upgradeRows = from row in upgradeTable.Select() 
                                  let ver = GetUpgradeScriptVersion(row["UPGRADEVERSION"].ToString()) 
                                  where ver.CompareTo(currentDbVersion) > 0
                                  orderby ver
                                  select row;

                int rowsCount = upgradeRows.Count();         
                string upgradeVersion = null;
                int count = 0;

                foreach (DataRow upgradeRow in upgradeRows)
                {
                    upgradeVersion = upgradeRow["UPGRADEVERSION"].ToString();

                    DisplayMessage(50609, ++count, rowsCount, upgradeVersion);
                    dBCommand.ExecuteScript("Upgrades." + upgradeRow["UPGRADESCRIPT"].ToString());
                    DisplayMessage(50607, upgradeVersion);
                }

                if (upgradeVersion != null)
                {
                    dBCommand.ExecuteNonQuery(string.Format(CultureInfo.CurrentCulture, setVersionSql, upgradeVersion));
                }
            }
        }

        /// <summary>
        /// Display message on and log it.
        /// </summary>
        /// <param name="messageStringId"></param>
        /// <param name="args"></param>
        private void DisplayMessage(int messageStringId, params object[] args)
        {
            Debug.Assert(progressForm != null);

            if (!progressForm.Visible)
                progressForm.Show();

            string message = ApplicationLocalizer.Language.Translate(messageStringId, args);
            progressForm.AddMessage(message);
            LogMessage(message);
        }

        /// <summary>
        /// Log a message to the file.
        /// </summary>
        /// <param name="message"></param>
        private void LogMessage(string message, params object[] args)
        {
            System.Diagnostics.Trace.TraceError(this.GetType().Name + ": " + string.Format(message, args));
        }

        #endregion

    }
}