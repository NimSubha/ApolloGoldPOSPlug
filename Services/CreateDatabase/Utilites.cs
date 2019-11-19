//Microsoft Dynamics AX for Retail POS Plug-ins 
//The following project is provided as SAMPLE code. Because this software is "as is," we may not provide support services for it.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Microsoft.Dynamics.Retail.Pos.CreateDatabaseService
{
    internal static class Utilites
    {

        #region Extension Methods

        /// <summary>
        /// Method to eat one sql command from Sql File
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetSqlCommands(this TextReader reader)
        {
            StringBuilder sqlCommand = new StringBuilder();

            while (reader.Peek() != -1)
            {
                string line = reader.ReadLine().Trim();

                if (line.Equals("GO", StringComparison.OrdinalIgnoreCase))
                {
                    yield return sqlCommand.ToString();
                    sqlCommand.Length = 0;
                }
                else
                    sqlCommand.AppendLine(line);
            }
        }

        /// <summary>
        /// Execute a resource sql script.
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <param name="resourceFileId"></param>
        public static void ExecuteScript(this DbCommand dBCommand, string scriptName)
        {
            using (TextReader sqlFile = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("CreateDatabase." + scriptName)))
            {
                foreach (string sqlCommand in sqlFile.GetSqlCommands())
                {
                    dBCommand.ExecuteNonQuery(sqlCommand);
                }
            }
        }

        /// <summary>
        /// Execute a text non-query command 
        /// </summary>
        /// <Remarks>CA2100 The string sqlStatement passed by the calling methods is parametrized.</Remarks>
        /// <param name="sql"></param>
        /// <param name="dbCommand"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "The string sqlStatement passed by the calling methods is parametrized.")]
        public static int ExecuteNonQuery(this DbCommand dBCommand, string sqlStatement)
        {
            dBCommand.CommandText = sqlStatement;

            return dBCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Execute a scalar query.
        /// </summary>
        /// <remarks>CA2100 The string sqlStatemetn passed by the calling methods is parametrized.</remarks>
        /// <param name="dBCommand"></param>
        /// <param name="sqlStatement"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "The string sqlStatemetn passed by the calling methods is parametrized.")]
        public static object ExecuteScalar(this DbCommand dBCommand, string sqlStatement)
        {
            dBCommand.CommandText = sqlStatement;

            return dBCommand.ExecuteScalar();
        }

        /// <summary>
        /// Try executing a scalar query.
        /// </summary>
        /// <param name="dBCommand"></param>
        /// <param name="sqlStatement"></param>
        /// <returns></returns>
        public static object TryExecuteScalar(this DbCommand dBCommand, string sqlStatement)
        {
            object result = null;

            try
            {
                result = dBCommand.ExecuteScalar(sqlStatement);
            }
            catch (SqlException)
            {
                // Ignore the any read error.
            }

            return result;
        }

        /// <summary>
        /// Open connection and begin a transaction.
        /// </summary>
        /// <param name="dBCommand"></param>
        /// <param name="dBConnection"></param>
        /// <returns></returns>
        public static DbTransaction BeginTransaction(this DbCommand dBCommand, DbConnection dBConnection)
        {
            dBCommand.Connection = dBConnection;
            if (dBConnection.State != ConnectionState.Open)
            {
                dBConnection.Open();
            }
            dBCommand.Transaction = dBConnection.BeginTransaction();

            return dBCommand.Transaction;
        }

        /// <summary>
        /// Tries the open db connection. If the database doesn't exists will retry 3 times with a second interval between retries.
        /// If the errors does not include "Can not open database ..." or the limit is reached it will throw.
        /// </summary>
        /// <param name="databaseConnection">The database connection to use.</param>
        public static void TryOpen(this DbConnection databaseConnection)
        {
            int numberOfTries = 0;
            int maxNumberOfTries = 3;

            while (numberOfTries < maxNumberOfTries)
            {
                try
                {
                    databaseConnection.Open();
                    numberOfTries = maxNumberOfTries;
                }
                catch (SqlException ex)
                {
                    numberOfTries++;
                    if (IsDatabaseDoesNotExistError(ex) && ((numberOfTries < maxNumberOfTries)))
                    {
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether [is database does not exist error] in [the specified exception].
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>
        ///     <c>true</c> if [is database does not exist error] in [the specified exception]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsDatabaseDoesNotExistError(SqlException exception)
        {
            bool databaseDoesNotExistsError = false;

            foreach (SqlError error in exception.Errors)
            {
                // Can not open database "xxx"; it means that the database does not exist or the user has no rights to access it.
                if (error.Number == 4060)
                {
                    databaseDoesNotExistsError = true;
                    break;
                }
            }

            return databaseDoesNotExistsError;
        }

        #endregion

        /// <summary>
        /// Creates the sql login for the specified windows username.
        /// </summary>
        /// <param name="dbCommand">The dbcommand with connection to the master db.</param>
        /// <param name="userName">The username.</param>
        /// <remarks>If the login already exists, the method is a noop.</remarks>
        public static void CreateSqlLoginForWindowsUser(DbCommand dbCommand, string userName)
        {
            // create the login if it doesn't already exist
            string statement = string.Format("SELECT name FROM sys.syslogins WHERE name LIKE '{0}'", userName);
            if (dbCommand.ExecuteScalar(statement) == null)
            {
                statement = string.Format("CREATE LOGIN \"{0}\" FROM WINDOWS", userName);
                dbCommand.ExecuteNonQuery(statement);
            }
        }


        /// <summary>
        /// Creates the sql user in the db specified by the dbCommand.
        /// </summary>
        /// <param name="dbCommand">The dbcommand with connection to the db where the user is to be created.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="loginName">The SQL login name.</param>
        /// <remarks>If the user already exists, the method is a noop.</remarks>
        public static void CreateSqlDBUserForSqlLogin(DbCommand dbCommand, string userName, string loginName)
        {
            string statement = string.Format("SELECT name from sys.database_principals where name LIKE '{0}'", userName);
            if (dbCommand.ExecuteScalar(statement) == null)
            {
                statement = string.Format("CREATE USER \"{0}\" FOR LOGIN \"{1}\"", userName, loginName);
                dbCommand.ExecuteNonQuery(statement);
            }
        }

        /// <summary>
        /// Adds the specified sql user to the roles in the db.
        /// </summary>
        /// <param name="dbCommand">The dbcommand with connection to the db on which the permissions are to be set.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="dbRoles">The listof existing db roles.</param>
        public static void AddSqlDBUserToDBRoles(DbCommand dbCommand, string userName, string[] dbRoles)
        {
            foreach (string dbRole in dbRoles)
            {
                string statement = string.Format("exec sp_addrolemember '{0}','{1}'", dbRole, userName);
                dbCommand.ExecuteNonQuery(statement);
            }
        }

        /// <summary>
        /// Grants the role the specified permissions. Creates the role if it doesn't exist.
        /// </summary>
        /// <param name="dbCommand">The dbcommand with connection to the db on which the role is to be configured.</param>
        /// <param name="roleName">The rolename.</param>
        /// <param name="permissions">The list of permissions to grant the role.</param>
        public static void CreateAndGrantPermissionsToSqlRole(DbCommand dbCommand, string roleName, string[] permissions)
        {
            string statement = string.Format("SELECT name from sysusers where name LIKE '{0}'", roleName);
            if (dbCommand.ExecuteScalar(statement) == null)
            {
                string permissionString = string.Join(",", permissions);
                statement = string.Format("CREATE ROLE {0}; GRANT {1} TO {0};", roleName, permissionString);
                dbCommand.ExecuteNonQuery(statement);
            }
            else
            {
                System.Diagnostics.Trace.TraceWarning("Database role '{0}' already exists. No changes have been made to it.", roleName);
            }
        }
    }
}
