/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Dynamics.Retail.Notification.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using System.Text;
using System.Data;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using LSRetailPosis.Transaction;

namespace Microsoft.Dynamics.Retail.Pos.Interaction
{
    [Export(typeof(IInteraction))]
    public class Interaction : IInteraction
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
            }
        }



        /// <summary>
        /// Used to make updates thread safe.
        /// </summary>
        static readonly object padlock = new object();

        //Start:nim
        enum CRWRetailDiscPermission // added on 04/05/2017
        {
            Cashier = 0,
            Salesperson = 1,
            Manager = 2,
            Other = 3,
            Manager2 = 4,
        }
        //End:Nim

        /// <summary>
        /// Catalog that holds the MEF parts that support the interation interface
        /// </summary>
        private static AggregateCatalog catalog;

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Method design")]
        [SuppressMessage("Microsoft.Reliability", "CA2000", Justification = "We must not dispose of the object as the static container has a dependency on them.")]
        public void InteractionRequest(InteractionRequestedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("InteractionRequestedEventArgs");
            }
            lock (padlock)
            {
                if (catalog == null)
                {
                    // Create the catalog of views

                    string appPath = LSRetailPosis.Settings.ApplicationSettings.GetAppPath();
                    string interactionDefaultPath = Path.Combine(appPath, "Services", "InteractionDefaults");
                    string defaultAssembly = Path.Combine(interactionDefaultPath, "InteractionDefaults.dll");
                    // The Extension directory under InteractionDefaults has been deprecated but has been left in for backward compatibility
                    string interactionExtensionPath = Path.Combine(interactionDefaultPath, "Extension");
                    string ExtensionPath = Path.Combine(appPath, "Extensions");

                    Assembly assembly = Assembly.LoadFrom(defaultAssembly);
                    catalog = new AggregateCatalog();

                    if (Directory.Exists(interactionExtensionPath))
                    {
                        catalog.Catalogs.Add(new DirectoryCatalog(ExtensionPath));
                        catalog.Catalogs.Add(new DirectoryCatalog(interactionExtensionPath));
                    }

                    catalog.Catalogs.Add(new AssemblyCatalog(assembly));
                }
            }

            string name = e.Context.GetType().Name;
            switch (name)
            {
                case "BarcodeConfirmation":
                    HandleBarcodeInteraction(e);
                    break;
                case "ExtendedLogOnConfirmation":
                    HandleExtendedLogOnInteraction(e);
                    break;
                case "DimensionConfirmation":
                    HandleDimensionInteraction(e);
                    break;
                case "InputConfirmation":
                    HandleInputInteraction(e);
                    break;
                case "LogOnConfirmation":
                    HandleLogOnInteraction(e);
                    break;
                case "ManagerAccessConfirmation":
                    HandleManagerAccessInteraction(e);
                    break;
                case "PayCashConfirmation":
                    HandlePayCashConfirmationInteraction(e);
                    break;
                case "PayCurrencyConfirmation":
                    HandleInputConfirmationInteraction(e);
                    break;
                case "PayCustomerAccountConfirmation":
                    HandlePayCustomerAccountConfirmationInteraction(e);
                    break;
                case "ProductInformationConfirmation":
                    HandleProductInformationInteraction(e);
                    break;
                case "ReturnTransactionConfirmation":
                    HandleReturnTransactionConfirmationInteraction(e);
                    break;
                case "RegisterTimeNotification":
                    HandleRegisterTimeNotificationInteraction(e);
                    break;
                case "ViewTimeClockEntriesNotification":
                    HandleViewTimeClockEntriesNotificationInteraction(e);
                    break;
                case "ProductDetailsConfirmation":
                    HandleProductDetailsConfirmation(e);
                    break;
                case "LoyaltyCardConfirmation":
                    HandleLoyaltyCardConfirmationInteraction(e);
                    break;
                case "RedeemLoyaltyPointsConfirmation":
                    HandleRedeemLoyaltyPointsConfirmationInteraction(e);
                    break;
                case "SaleRefundsDisplayNotification":
                    HandleSaleRefundsDisplayNotification(e);
                    break;
                case "DisbursementSlipCreationConfirmation":
                    HandleDisbursementSlipCreationConfirmation(e);
                    break;
                default:
                    throw new InvalidDataException(string.Format("Invalid confirmation name '{0}'.", name));
            }

            e.Callback();
        }

        /// <summary>
        /// Loads and shows the view then returns results.
        /// </summary>
        /// <typeparam name="TParam">Type of parameter passed into view's constructor</typeparam>
        /// <typeparam name="TResults">Type of results returned</typeparam>
        /// <param name="viewName">Name of view to look up</param>
        /// <param name="context">The value of the parameter to pass to the view's constructor</param>
        /// <param name="showDialog">Show dialog when value true otherwise hide dialog</param>
        /// <returns></returns>
        private TResults InvokeInteraction<TParam, TResults>(string viewName, TParam context, bool showDialog)
            where TParam : Microsoft.Practices.Prism.Interactivity.InteractionRequest.Notification
            where TResults : class, new()
        {
            IInteractionView view = null;

            using (CompositionContainer container = new CompositionContainer(catalog))
            {
                // Add context to container for satisfying ImportingConstructor
                container.ComposeExportedValue<TParam>(context);

                // Load the view. If no or more than one view satisfying the condition is found, throws InvalidOperationException
                view = container.GetExportedValues<IInteractionView>(viewName).First();

                // Args to create the form are currently passed to single param ctor. If default ctor is used (without params) uncomment following line:
                // view.Initialize(e.Context); 

                // Show form
                System.Windows.Forms.Form form = (System.Windows.Forms.Form)view;
                if (showDialog)
                {
                    this.Application.ApplicationFramework.POSShowForm(form);
                }

                // Get results
                return view.GetResults<TResults>();
            }
        }

        #region Interaction handlers

        // Dimension form
        private void HandleDimensionInteraction(InteractionRequestedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("InteractionRequestedEventArgs");
            }

            DimensionConfirmation context = (DimensionConfirmation)e.Context;
            DimensionConfirmation results = InvokeInteraction<DimensionConfirmation, DimensionConfirmation>("DimensionView", context, context.DisplayDialog);

            if (results != null)
            {
                context.Confirmed = results.Confirmed;
                context.SelectDimCombination = results.SelectDimCombination;
            }
        }

        // Barcode form
        private void HandleBarcodeInteraction(InteractionRequestedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("InteractionRequestedEventArgs");
            }

            BarcodeConfirmation context = (BarcodeConfirmation)e.Context;
            BarcodeConfirmation results = InvokeInteraction<BarcodeConfirmation, BarcodeConfirmation>("BarcodeForm", context, true);

            if (results != null)
            {
                context.Confirmed = results.Confirmed;
                context.SelectedBarcodeId = results.SelectedBarcodeId;
            }
        }

        private void HandleExtendedLogOnInteraction(InteractionRequestedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("InteractionRequestedEventArgs");
            }

            ExtendedLogOnConfirmation context = (ExtendedLogOnConfirmation)e.Context;
            ExtendedLogOnConfirmation results = InvokeInteraction<ExtendedLogOnConfirmation, ExtendedLogOnConfirmation>("ExtendedLogOnForm", context, true);

            if (results != null)
            {
                context.Confirmed = results.Confirmed;
            }
        }

        // Input form
        private void HandleInputInteraction(InteractionRequestedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("InteractionRequestedEventArgs");
            }

            InputConfirmation context = (InputConfirmation)e.Context;
            InputConfirmation results = InvokeInteraction<InputConfirmation, InputConfirmation>("InputForm", context, true);

            if (results != null)
            {
                context.Confirmed = results.Confirmed;
                context.EnteredText = results.EnteredText;
            }
        }

        private void HandleLogOnInteraction(InteractionRequestedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("InteractionRequestedEventArgs");
            }

            LogOnConfirmation context = (LogOnConfirmation)e.Context;
            LogOnConfirmation results = InvokeInteraction<LogOnConfirmation, LogOnConfirmation>("LogOnForm", context, true);

            if (results != null)
            {
                context.Confirmed = results.Confirmed;
                context.LogOnStatus = results.LogOnStatus;
            }
        }

        private void HandleManagerAccessInteraction(InteractionRequestedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("InteractionRequestedEventArgs");
            }

            ManagerAccessConfirmation context = (ManagerAccessConfirmation)e.Context;
            ManagerAccessConfirmation results = InvokeInteraction<ManagerAccessConfirmation, ManagerAccessConfirmation>("ManagerAccessForm", context, true);

            if (results != null)
            {
                context.Confirmed = results.Confirmed;
                context.OperatorId = results.OperatorId;
            }
        }

        private void HandleInputConfirmationInteraction(InteractionRequestedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("InteractionRequestedEventArgs");
            }

            PayCurrencyConfirmation context = (PayCurrencyConfirmation)e.Context;
            PayCurrencyConfirmation results = InvokeInteraction<PayCurrencyConfirmation, PayCurrencyConfirmation>("PayCurrencyView", context, true);

            if (results != null)
            {
                context.Confirmed = results.Confirmed;
                context.RegisteredAmount = results.RegisteredAmount;
                context.ExchangeRate = results.ExchangeRate;
                context.CurrentCurrencyCode = results.CurrentCurrencyCode;
            }
        }

        private void HandlePayCashConfirmationInteraction(InteractionRequestedEventArgs e)
        {
            //Base
            /*if (e == null)
            {
                throw new ArgumentNullException("InteractionRequestedEventArgs");
            }

            PayCashConfirmation context = (PayCashConfirmation)e.Context;
            PayCashConfirmation results = InvokeInteraction<PayCashConfirmation, PayCashConfirmation>("PayCashForm", context, true);

            if (results != null)
            {
                context.Confirmed = results.Confirmed;
                context.RegisteredAmount = results.RegisteredAmount;
                context.OperationDone = results.OperationDone;
            }*/

            //Start:Nim
            if (e == null)
            {
                throw new ArgumentNullException("InteractionRequestedEventArgs");
            }

            PayCashConfirmation context = (PayCashConfirmation)e.Context;
            //bool bIsgv = false;// Nimbus
            //decimal dRegAmount = context.RegisteredAmount;// Nimbus

            PropertyInfo[] properties = context.TenderInfo.GetType().GetProperties();

            //foreach(PropertyInfo property in properties) // Nimbus
            //{
            //    if(property.Name == "TenderID")
            //    {
            //        object value = context.TenderInfo.GetType().GetProperty(property.Name).GetValue(context.TenderInfo, null);
            //        if(value != null)
            //        {
            //            if(isGVPayment(Convert.ToInt16(value)))
            //                bIsgv = true;
            //            else
            //                bIsgv = false;
            //        }
            //    }
            //}
            PayCashConfirmation results;//= InvokeInteraction<PayCashConfirmation, PayCashConfirmation>("PayCashForm", context, true);


            results = InvokeInteraction<PayCashConfirmation, PayCashConfirmation>("PayCashForm", context, true);


            //if(Microsoft.Dynamics.Retail.Pos.Contracts.PosisOperations.PayCheque)
            //{

            //}
            //Microsoft.Dynamics.Retail.Pos.Contracts.PosisOperations.PayCheque
            string sOp = "";
            bool bIsSaleReturnToAdvance = false;
            foreach (PropertyInfo property in properties) // Nimbus
            {
                if (property.Name == "TenderID")
                {
                    object value = ((LSRetailPosis.Transaction.Tender)(context.TenderInfo)).PosisOperation;
                    if (Convert.ToString(value) == "PayCheque")
                    {
                        sOp = "PayCheque";
                    }

                    //=============================Soutik=============================================
                    bIsSaleReturnToAdvance = isSRToAdvance(Convert.ToInt16(((LSRetailPosis.Transaction.Tender)(context.TenderInfo)).TenderID));
                }
            }

            if (sOp != "PayCheque")
            {
                if (results != null)
                {
                    if (results != null)
                    {
                        string tblDAILYCASHPAYNEG = "DAILYCASHPAYNEG" + ApplicationSettings.Terminal.TerminalId;
                        decimal dDailyNegCashLimit = Math.Abs(GetTotNegCashPayment(tblDAILYCASHPAYNEG)); //transaction value

                        string sTblName = "NEGCASHPAY" + ApplicationSettings.Terminal.TerminalId;
                        string sOGPTable = "OGPCASHPAY" + ApplicationSettings.Terminal.TerminalId;
                       
                        decimal dCashLimit = GetCashPaymentLimitValue();//param value
                        decimal dLineCash = Math.Abs(GetTotCashPayment(sTblName));


                        decimal dOGPLineCash = Math.Abs(GetTotCashPayment(sOGPTable));
                        bool isOGPTrans = OGPCashPayTransType(sOGPTable);
                        decimal dOGPCashPayLimit = GetOGPurchaseMaxCashLimit();

                        if (results.RegisteredAmount < 0)
                        {
                            if (isOGPTrans)
                            {
                                if (Math.Abs(results.RegisteredAmount) <= (dOGPCashPayLimit - dOGPLineCash))
                                {
                                    TempTableCreate(sOGPTable);
                                    UpdateTransCashInfo(sOGPTable, ApplicationSettings.Terminal.TerminalId, results.RegisteredAmount);

                                    context.Confirmed = results.Confirmed;
                                    context.RegisteredAmount = results.RegisteredAmount;
                                    context.OperationDone = results.OperationDone;
                                }
                                else
                                {
                                    if (!bIsSaleReturnToAdvance)
                                    {
                                        results.Confirmed = false;
                                        MessageBox.Show("Exceeding the cash payment limit.");
                                    }
                                    else
                                    {
                                        TempTableCreate(sOGPTable);
                                        UpdateTransCashInfo(sOGPTable, ApplicationSettings.Terminal.TerminalId, results.RegisteredAmount);

                                        context.Confirmed = results.Confirmed;
                                        context.RegisteredAmount = results.RegisteredAmount;
                                        context.OperationDone = results.OperationDone;
                                    }
                                }
                            }
                            else
                            {

                                if (Math.Abs(results.RegisteredAmount) <= Math.Abs((dCashLimit - dLineCash - dDailyNegCashLimit)))
                                {
                                    TempTableCreate(sTblName);
                                    UpdateTransCashInfo(sTblName, ApplicationSettings.Terminal.TerminalId, results.RegisteredAmount);

                                    context.Confirmed = results.Confirmed;
                                    context.RegisteredAmount = results.RegisteredAmount;
                                    context.OperationDone = results.OperationDone;
                                }
                                else
                                {
                                    if (!bIsSaleReturnToAdvance)
                                    {
                                        results.Confirmed = false;
                                        MessageBox.Show("Exceeding the cash payment limit.");
                                    }
                                    else
                                    {
                                        TempTableCreate(sTblName);
                                        UpdateTransCashInfo(sTblName, ApplicationSettings.Terminal.TerminalId, results.RegisteredAmount);

                                        context.Confirmed = results.Confirmed;
                                        context.RegisteredAmount = results.RegisteredAmount;
                                        context.OperationDone = results.OperationDone;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //context.Confirmed = results.Confirmed;
                            //context.RegisteredAmount = results.RegisteredAmount;
                            //context.OperationDone = results.OperationDone;
                            string sTableName = "DAILYCASHPAY" + ApplicationSettings.Terminal.TerminalId;
                            decimal dDailyCashLimit = GetCashPaymentLimitValuePositive();
                            decimal dTransLineCash = Math.Abs(GetTotCashPayment(sTableName));

                            if (Math.Abs(results.RegisteredAmount) <= ((dDailyCashLimit - dTransLineCash)))
                            {
                                TempTableCreate(sTableName);
                                UpdateTransCashInfo(sTableName, ApplicationSettings.Terminal.TerminalId, results.RegisteredAmount);

                                context.Confirmed = results.Confirmed;
                                context.RegisteredAmount = results.RegisteredAmount;
                                context.OperationDone = results.OperationDone;
                            }
                            else
                            {
                                results.Confirmed = false;
                                MessageBox.Show("Exceeding the daily cash transaction limit.");
                            }
                        }

                    }
                }
            }
            else
            {
                context.Confirmed = results.Confirmed;
                context.RegisteredAmount = results.RegisteredAmount;
                context.OperationDone = results.OperationDone;
            }
            //if(results != null)
            //{
            //    context.Confirmed = results.Confirmed;
            //    context.RegisteredAmount = results.RegisteredAmount;
            //    context.OperationDone = results.OperationDone;
            //}
            //End:Nim
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

        //Start:Nim
        private decimal GetNegCashPayment(string sTable)
        {
            decimal dValue = 0M;
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = string.Empty;
            commandText = " IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTable + "')" +
                        " BEGIN  SELECT sum(isnull(NEGAMT,0)) FROM " +
                        " " + sTable + " WHERE Terminalid = '" + ApplicationSettings.Database.TerminalID + "' END";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            return dValue = Convert.ToDecimal(command.ExecuteScalar());

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

        private int GetSRCashPayment(string sTable)
        {
            int dValue = 0;
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = string.Empty;
            commandText = " IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTable + "')" +
                        " BEGIN  SELECT isnull(SRCASH,0) FROM " +
                        " " + sTable + " WHERE Terminalid = '" + ApplicationSettings.Database.TerminalID + "' END";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            return dValue = Convert.ToInt16(command.ExecuteScalar());

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
            commandText.Append(" TERMINALID NVARCHAR (20),NEGAMT NUMERIC (20,2),SRCASH INT NOT NULL DEFAULT 0,IsTransAmt INT NOT NULL DEFAULT 0) END");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            command.ExecuteNonQuery();

            if (conn.State == ConnectionState.Open)
                conn.Close();

        }

        private void UpdateNegCashIfo(string sTableName, string sTerminalId, decimal dEmpAmt)
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();


            commandText.Append("IF EXISTS (SELECT A.NAME FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTableName + "')");
            commandText.Append(" BEGIN  INSERT INTO  " + sTableName + "  VALUES('" + sTerminalId + "',");
            commandText.Append(" '" + dEmpAmt + "',0) END");


            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            command.ExecuteNonQuery();
            if (conn.State == ConnectionState.Open)
                conn.Close();

        }

        private void dropTempTable(string sTableName) 
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder sbQuery = new StringBuilder();
            sbQuery.Append(" IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTableName + "')");
            sbQuery.Append(" drop table " + sTableName + "");
            DataTable dtGSS = new DataTable();
            if (conn.State == ConnectionState.Closed)
                conn.Open();
          
            SqlCommand command = new SqlCommand(sbQuery.ToString(), conn);
            command.CommandTimeout = 0;
            command.ExecuteScalar();


            if (conn.State == ConnectionState.Open)
                conn.Close();
        }

        private DataTable GetGVIfo(string sTerminalId, SqlTransaction sqlTransaction = null)
        {
            string sTblName = "EMIINFO" + ApplicationSettings.Terminal.TerminalId;

            DataTable dtCard = new DataTable();
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTblName + "')");
            commandText.Append(" BEGIN  SELECT ISNULL(TRANSACTIONID,'') AS TRANSACTIONID,ISNULL(TERMINALID,'') AS TERMINALID,");
            commandText.Append(" ISNULL(EMPCODE,'') AS EMPCODE, ISNULL(EMIORDERNO,'') AS EMIORDERNO,ISNULL(EMIAMT,0) EMIAMT FROM ");
            commandText.Append(" " + sTblName + " WHERE Terminalid = '" + sTerminalId + "' END");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            if (sqlTransaction == null)
            {
                using (SqlCommand command = new SqlCommand(commandText.ToString(), conn))
                {
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dtCard);
                }
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            else
            {
                using (SqlCommand command = new SqlCommand(commandText.ToString(), sqlTransaction.Connection, sqlTransaction))
                {
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dtCard);
                }
            }

            return dtCard;
        }

        private decimal GetCustomerLimit(string sCustAcc)
        {
            decimal bResult = 0m;
            if (sCustAcc != null)
            {
                ReadOnlyCollection<object> containerArraySR;
                if (!string.IsNullOrEmpty(sCustAcc))
                {
                    try
                    {
                        if (this.Application.TransactionServices.CheckConnection())
                        {
                            containerArraySR = this.Application.TransactionServices.InvokeExtension("GetCustomerBalance", sCustAcc);
                            bResult = Convert.ToDecimal(containerArraySR[3]);
                        }
                    }
                    catch (Exception)
                    {
                        bResult = 0;
                    }
                }
            }
            return bResult;
        }

        private decimal GetCashPaymentLimitValue(RetailTransaction retailTrans)
        {
            decimal dTransCash = 0m;
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

            sQuery = "GETDAILYCASHPAYFORACUSTOMER";

            SqlCommand command = new SqlCommand(sQuery, conn);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@Store", SqlDbType.NVarChar).Value = sStoreNo;
            command.Parameters.Add("@CUSTACCOUNT", SqlDbType.NVarChar).Value = retailTrans.Customer.CustomerId;
            command.Parameters.Add("@Terminal", SqlDbType.NVarChar).Value = retailTrans.TerminalId;
            command.Parameters.Add("@TransactionId", SqlDbType.NVarChar).Value = retailTrans.TransactionId;
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
        //End:Nim

        private void HandlePayCustomerAccountConfirmationInteraction(InteractionRequestedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("InteractionRequestedEventArgs");
            }

            PayCustomerAccountConfirmation context = (PayCustomerAccountConfirmation)e.Context;
            PayCustomerAccountConfirmation results = InvokeInteraction<PayCustomerAccountConfirmation, PayCustomerAccountConfirmation>("PayCustomerAccountForm", context, true);

            if (results != null)
            {
                context.Confirmed = results.Confirmed;//base
                context.RegisteredAmount = results.RegisteredAmount;//base
                context.CustomerId = results.CustomerId;//base

                #region Commented
                /*if (results.RegisteredAmount < 0)//added on 280119
                {
                    context.Confirmed = results.Confirmed;//base
                    context.RegisteredAmount = results.RegisteredAmount;//base
                    context.CustomerId = results.CustomerId;//base
                }
                else
                {
                    decimal dCreditLimit = 0m;
                    dCreditLimit = GetCustomerLimit(results.CustomerId);
                    if (Math.Abs(results.RegisteredAmount) <= (dCreditLimit))// + results.RegisteredAmount
                    {
                        context.Confirmed = results.Confirmed;
                        context.RegisteredAmount = results.RegisteredAmount;
                        context.OperationDone = results.OperationDone;
                    }
                    else
                    {
                        results.Confirmed = false;
                        MessageBox.Show("Exceeding the pay customer payment limit.");
                    }
                }*/
                #endregion
            }
        }

        private void HandleProductInformationInteraction(InteractionRequestedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("InteractionRequestedEventArgs");
            }

            ProductInformationConfirmation context = (ProductInformationConfirmation)e.Context;
            ProductInformationConfirmation results = InvokeInteraction<ProductInformationConfirmation, ProductInformationConfirmation>("ProductInformationForm", context, true);

            if (results != null)
            {
                context.Confirmed = results.Confirmed;
            }
        }

        private void HandleReturnTransactionConfirmationInteraction(InteractionRequestedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("InteractionRequestedEventArgs");
            }

            ReturnTransactionConfirmation context = (ReturnTransactionConfirmation)e.Context;
            ReturnTransactionConfirmation results = InvokeInteraction<ReturnTransactionConfirmation, ReturnTransactionConfirmation>("ReturnTransactionForm", context, true);

            if (results != null)
            {
                context.Confirmed = results.Confirmed;
                context.ReturnedLineNumbers = results.ReturnedLineNumbers;
            }
        }

        private void HandleRegisterTimeNotificationInteraction(InteractionRequestedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("InteractionRequestedEventArgs");
            }

            RegisterTimeNotification context = (RegisterTimeNotification)e.Context;
            RegisterTimeNotification results = InvokeInteraction<RegisterTimeNotification, RegisterTimeNotification>("RegisterTimeForm", context, true);
        }

        private void HandleViewTimeClockEntriesNotificationInteraction(InteractionRequestedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("InteractionRequestedEventArgs");
            }

            ViewTimeClockEntriesNotification context = (ViewTimeClockEntriesNotification)e.Context;
            ViewTimeClockEntriesNotification results = InvokeInteraction<ViewTimeClockEntriesNotification, ViewTimeClockEntriesNotification>("ViewTimeClockEntriesForm", context, true);
        }

        private void HandleProductDetailsConfirmation(InteractionRequestedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("InteractionRequestedEventArgs");
            }

            ProductDetailsConfirmation context = (ProductDetailsConfirmation)e.Context;
            ProductDetailsConfirmation results = InvokeInteraction<ProductDetailsConfirmation, ProductDetailsConfirmation>("ProductDetailsForm", context, true);

            if (results != null)
            {
                context.Confirmed = results.Confirmed;
                context.AddToSale = results.AddToSale;
            }
        }

        private void HandleLoyaltyCardConfirmationInteraction(InteractionRequestedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("InteractionRequestedEventArgs");
            }

            LoyaltyCardConfirmation context = (LoyaltyCardConfirmation)e.Context;
            LoyaltyCardConfirmation results = InvokeInteraction<LoyaltyCardConfirmation, LoyaltyCardConfirmation>("LoyaltyCardForm", context, true);

            if (results != null)
            {
                context.Confirmed = results.Confirmed;
            }
        }

        private void HandleRedeemLoyaltyPointsConfirmationInteraction(InteractionRequestedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("InteractionRequestedEventArgs");
            }

            RedeemLoyaltyPointsConfirmation context = (RedeemLoyaltyPointsConfirmation)e.Context;
            RedeemLoyaltyPointsConfirmation results = InvokeInteraction<RedeemLoyaltyPointsConfirmation, RedeemLoyaltyPointsConfirmation>("RedeemLoyaltyPointsForm", context, true);

            if (results != null)
            {
                context.Confirmed = results.Confirmed;
                context.CardNumber = results.CardNumber;
                context.DiscountAmount = results.DiscountAmount;
            }
        }

        private void HandleSaleRefundsDisplayNotification(InteractionRequestedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            SaleRefundsDisplayNotification context = (SaleRefundsDisplayNotification)e.Context;
            InvokeInteraction<SaleRefundsDisplayNotification, SaleRefundsDisplayNotification>("SaleRefundDetailsForm", context, true);
        }

        private void HandleDisbursementSlipCreationConfirmation(InteractionRequestedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            DisbursementSlipCreationConfirmation context = (DisbursementSlipCreationConfirmation)e.Context;
            DisbursementSlipCreationConfirmation results = InvokeInteraction<DisbursementSlipCreationConfirmation, DisbursementSlipCreationConfirmation>("DisbursementSlipCreationForm", context, true);

            if (results != null)
            {
                context.Confirmed = results.Confirmed;
                context.DisbursementSlipInfo = results.DisbursementSlipInfo;
            }
        }

        #endregion

        //Start:Nim
        private decimal GetCashPaymentLimitValue()
        {
            decimal dValue = 0M;
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = string.Empty;
            commandText = "SELECT isnull(LIMITFORCASHPAY,0) LIMITFORCASHPAY FROM [RETAILSTORETABLE] WHERE STORENUMBER = '" + ApplicationSettings.Database.StoreID + "' ";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            return dValue = Convert.ToDecimal(command.ExecuteScalar());

        }

        private decimal GetCashPaymentLimitValuePositive()
        {
            decimal dValue = 0M;
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = string.Empty;
            commandText = "SELECT isnull(MAXINVOICEAMOUNT,0) MAXINVOICEAMOUNT FROM [RETAILPARAMETERS] WHERE DATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "' ";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            return dValue = Convert.ToDecimal(command.ExecuteScalar());

        }

        private decimal GetTotCashPayment(string sTable)
        {
            decimal dValue = 0M;
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = string.Empty;
            commandText = " IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTable + "')" +
                        " BEGIN  SELECT isnull(sum(isnull(NEGAMT,0)),0) FROM " +
                        " " + sTable + " WHERE Terminalid = '" + ApplicationSettings.Database.TerminalID + "' END";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            return dValue = Convert.ToDecimal(command.ExecuteScalar());

        }

        private bool OGPCashPayTransType(string sTable)  
        {
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = string.Empty;
            commandText = " IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTable + "')" +
                        " BEGIN  SELECT top 1 SRCASH FROM " +
                        " " + sTable + " WHERE Terminalid = '" + ApplicationSettings.Database.TerminalID + "' and SRCASH=2 END";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            int iValue = Convert.ToInt16(command.ExecuteScalar());
            if (iValue == 2)
                return true;
            else
                return false;

        }

        private decimal GetTotNegCashPayment(string sTable) 
        {
            decimal dValue = 0M;
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = string.Empty;
            commandText = " IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTable + "')" +
                        " BEGIN  SELECT isnull(sum(isnull(NEGAMT,0)),0) FROM " +
                        " " + sTable + " WHERE Terminalid = '" + ApplicationSettings.Database.TerminalID + "' END";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            return dValue = Convert.ToDecimal(command.ExecuteScalar());

        }

        private void UpdateTransCashInfo(string sTableName, string sTerminalId, decimal dEmpAmt)
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();


            commandText.Append("IF EXISTS (SELECT A.NAME FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTableName + "')");
            commandText.Append(" BEGIN  INSERT INTO  " + sTableName + "  VALUES('" + sTerminalId + "',");
            commandText.Append(" '" + dEmpAmt + "',0,1) END");


            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            command.ExecuteNonQuery();
            if (conn.State == ConnectionState.Open)
                conn.Close();

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
        //End:Nim
    }
}