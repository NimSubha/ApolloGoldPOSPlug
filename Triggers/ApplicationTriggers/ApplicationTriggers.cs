//Microsoft Dynamics AX for Retail POS Plug-ins 
//The following project is provided as SAMPLE code. Because this software is "as is," we may not provide support services for it.

using System;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using LSRetailPosis;
using LSRetailPosis.Settings;
using LSRetailPosis.Settings.FunctionalityProfiles;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.Triggers;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using Microsoft.Dynamics.Retail.Pos.BlankOperations;
using System.IO;

namespace Microsoft.Dynamics.Retail.Pos.ApplicationTriggers
{
    [Export(typeof(IApplicationTrigger))]
    public class ApplicationTriggers : IApplicationTrigger
    {

        /// <summary>
        /// IApplication instance.
        /// </summary>
        private IApplication application;


        /// <summary>
        /// Gets or sets the IApplication instance.
        /// </summary>
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
                InternalApplication = value;
            }
        }

        /// <summary>
        /// Gets or sets the static IApplication instance.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal static IApplication InternalApplication { get; private set; }

        public ApplicationTriggers()
        {
        }

        #region IApplicationTriggers Members

        public void ApplicationStart()
        {
            string source = "IApplicationTriggers.ApplicationStart";
            string value = "Application has started";
            LSRetailPosis.ApplicationLog.Log(source, value, LSRetailPosis.LogTraceLevel.Debug);
            LSRetailPosis.ApplicationLog.WriteAuditEntry(source, value);


            #region "VERSION HANDLIG WORKING"
            string strGetversion = String.Format("SELECT DLLNAME, DLLVERSION FROM DLLVERSION WHERE ACTIVE=1 ");
            if (LSRetailPosis.Settings.ApplicationSettings.Database.LocalConnection.State != ConnectionState.Open) { LSRetailPosis.Settings.ApplicationSettings.Database.LocalConnection.Open(); }
            System.Data.SqlClient.SqlCommand cmdGetversion = new System.Data.SqlClient.SqlCommand(strGetversion, LSRetailPosis.Settings.ApplicationSettings.Database.LocalConnection);
            System.Data.SqlClient.SqlDataAdapter sqlda = new System.Data.SqlClient.SqlDataAdapter(cmdGetversion);
            System.Data.DataTable dtVer = new System.Data.DataTable();
            sqlda.Fill(dtVer);
            Boolean isError = false;
            if (dtVer.Rows.Count > 0)
            {
                foreach (System.Data.DataRow dr in dtVer.Rows)
                {
                    isError = false;
                    String dllServicesPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Services\\" + dr["DLLNAME"].ToString() + ".dll";
                    String dllTriggerPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Triggers\\" + dr["DLLNAME"].ToString() + ".dll";
                    String dllServicesIDPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Services\\InteractionDefaults\\" + dr["DLLNAME"].ToString() + ".dll";
                    
                    string dllPath = "";

                    if (File.Exists(@"" + dllServicesPath + ""))
                    {
                        dllPath = dllServicesPath;
                    }
                    else if (File.Exists(@"" + dllTriggerPath + ""))
                    {
                        dllPath = dllTriggerPath;
                    }
                    else if (File.Exists(@"" + dllServicesIDPath + ""))
                    {
                        dllPath = dllServicesIDPath;
                    }

                    try
                    {
                        Assembly asVer = Assembly.LoadFile(dllPath);
                        if (asVer != null)
                        {
                            String asFullName = asVer.FullName;
                            VersionHandler sourceFile = new VersionHandler(asFullName);


                            var result = sourceFile.dllVersion.CompareTo(new Version(dr["DLLVERSION"].ToString()));

                            if (result != 0)
                            {

                                using (var form = new LSRetailPosis.POSProcesses.frmMessage("DLL Version Mismatch in " + sourceFile.dllName + ".dll file.Please contact administrator !!", MessageBoxButtons.OK, MessageBoxIcon.Error))
                                {
                                    isError = true;
                                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(form);
                                    throw new LSRetailPosis.PosStartupException("DLL Version Mismatch.Please Contact Administrator !!");
                                }

                            }
                        }
                    }
                    catch (Exception)
                    {
                        if (!isError)
                        {
                            using (var form = new LSRetailPosis.POSProcesses.frmMessage(dr["DLLNAME"].ToString() + ".dll file not found in folder.Please contact administrator !!", MessageBoxButtons.OK, MessageBoxIcon.Error))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(form);
                                throw new LSRetailPosis.PosStartupException(dr["DLLNAME"].ToString() + ".dll file not found in folder.Please contact administrator !!");
                            }
                        }
                        else { throw; }
                    }
                }
            }
            #endregion

            // If the store is in Brazil, we should only allow to run the POS if the Functionality profile's ISO locale is Brazil
            if (ApplicationSettings.Terminal.StoreCountry.Equals(SupportedCountryRegion.BR.ToString(), StringComparison.OrdinalIgnoreCase)
                && Functions.CountryRegion != SupportedCountryRegion.BR)
            {
                var message = ApplicationLocalizer.Language.Translate(85082, // The locale must be Brazil. In the POS functionality profile form, on the General tab, in the Locale field, select Brazil.
                                                                      ApplicationSettings.Terminal.StoreCountry);

                using (var form = new LSRetailPosis.POSProcesses.frmMessage(message, MessageBoxButtons.OK, MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(form);
                }

                throw new LSRetailPosis.PosStartupException(message);
            }
        }

        public void ApplicationStop()
        {
            string source = "IApplicationTriggers.ApplicationStop";
            string value = "Application has stopped";
            LSRetailPosis.ApplicationLog.Log(source, value, LSRetailPosis.LogTraceLevel.Debug);
            LSRetailPosis.ApplicationLog.WriteAuditEntry(source, value);
        }

        public void PostLogon(bool loginSuccessful, string operatorId, string name)
        {
            string source = "IApplicationTriggers.PostLogon";
            string value = loginSuccessful ? "User has successfully logged in. OperatorID: " + operatorId : "Failed user login attempt. OperatorID: " + operatorId;
            LSRetailPosis.ApplicationLog.Log(source, value, LSRetailPosis.LogTraceLevel.Debug);
            LSRetailPosis.ApplicationLog.WriteAuditEntry(source, value);
        }

        public void PreLogon(IPreTriggerResult preTriggerResult, string operatorId, string name)
        {
            // LSRetailPosis.ApplicationLog.Log("IApplicationTriggers.PreLogon", "Before the user has been logged on...", LSRetailPosis.LogTraceLevel.Trace);

            //Start: Nim
            if (!isClientServerDateMatched())
            {
                string message = "Server date and local terminal date is mismatch. ";
                using (var form = new LSRetailPosis.POSProcesses.frmMessage(message, MessageBoxButtons.OK, MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(form);
                }

                throw new LSRetailPosis.PosStartupException(message);
            }
            else
            {
                //base
                LSRetailPosis.ApplicationLog.Log("IApplicationTriggers.PreLogon", "Before the user has been logged on...", LSRetailPosis.LogTraceLevel.Trace);
            }
            //End Nim
        }

        public void Logoff(string operatorId, string name)
        {
            string source = "IApplicationTriggers.Logoff";
            string value = "User has successfully logged off. OperatorID: " + operatorId;
            LSRetailPosis.ApplicationLog.Log(source, value, LSRetailPosis.LogTraceLevel.Debug);
            LSRetailPosis.ApplicationLog.WriteAuditEntry(source, value);
        }

        public void LoginWindowVisible()
        {
            LSRetailPosis.ApplicationLog.Log("IApplicationTriggers.LoginWindowVisible", "When the login window is visible", LSRetailPosis.LogTraceLevel.Trace);
        }

        #endregion

        //Start: Nim
        private bool isClientServerDateMatched()
        {
            bool bMacthed = true;

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = " SELECT GETDATE() ";

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            string sServerDate = Convert.ToString(command.ExecuteScalar());
            if (connection.State == ConnectionState.Open)
                connection.Close();

            if (Convert.ToDateTime(sServerDate).ToShortDateString() != DateTime.Now.ToShortDateString())
                bMacthed = false;

            return bMacthed;
        }
        //End:Nim
    }
}
