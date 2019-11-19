/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using LSRetailPosis;
using LSRetailPosis.POSProcesses.Common;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Notification.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.BusinessLogic;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace Microsoft.Dynamics.Retail.Pos.LogOn
{
    /// <summary>
    /// Log on service implementation.
    /// </summary>
    [Export(typeof(ILogOn))]
    public sealed class LogOn : ILogOn
    {

        #region Properties

        /// <summary>
        /// IApplication instance.
        /// </summary>
        [Import]
        public IApplication Application { get; set; }

        #endregion

        #region ILogOn

        /// <summary>
        /// Initiate logOn flow and returns the status.
        /// </summary>
        /// <returns>Log on status.</returns>
        public LogOnStatus LogOnOperator()
        {
            LogOnStatus logonStatus = LogOnStatus.None;

            // Automation Logon (logon using command line user/password -SU<operatorId> -SP<password>).
            if (!string.IsNullOrWhiteSpace(ApplicationSettings.StartUser) && !string.IsNullOrWhiteSpace(ApplicationSettings.StartPassword))
            {
                if (Login.LogOnUser(true, ApplicationSettings.StartUser, string.Empty, string.Empty, ApplicationSettings.StartPassword))
                {
                    logonStatus = LogOnStatus.LogOn;
                }
            }

            // If not auto logged on, then display the UI.
            if (logonStatus != LogOnStatus.LogOn)
            {
                // Invoke Trigger
                foreach (var trigger in Application.Triggers.ApplicationTriggers)
                {
                    trigger.LoginWindowVisible();
                }

                LogOnConfirmation logOnConfirmation = new LogOnConfirmation();

                InteractionRequestedEventArgs request = new InteractionRequestedEventArgs(logOnConfirmation, () =>
                {
                    if (logOnConfirmation.Confirmed)
                    {
                        logonStatus = (LogOnStatus)logOnConfirmation.LogOnStatus;
                    }
                }
                );

                Application.Services.Interaction.InteractionRequest(request);
            }

            return logonStatus;
        }

        /// <summary>
        /// Verifies if the operator has access for a given operation.
        /// </summary>
        /// <param name="operatorID">The operator ID.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>
        /// True if operator has access, false otherwise.
        /// </returns>
        public bool VerifyOperationAccess(string operatorID, PosisOperations operation)
        {
            return VerifyOperationAccess(operatorID, operation, null);
        }

        /// <summary>
        /// Verifies if the transaction operator has access for a given operation.
        /// </summary>
        /// <param name="posTransaction">The pos transaction.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>
        /// True if operator has access, false otherwise.
        /// </returns>
        /// <remarks>Use this method when transaction is available.</remarks>
        public bool VerifyOperationAccess(IPosTransaction posTransaction, PosisOperations operation)
        {
            if (posTransaction == null)
            {
                throw new ArgumentNullException("posTransaction");
            }

            return VerifyOperationAccess(posTransaction.OperatorId, operation, posTransaction.TransactionId);
        }

        /// <summary>
        /// Initiate flow for extended log on assignment.
        /// </summary>
        public void ExtendedLogOns()
        {
            ExtendedLogOnConfirmation assignExtendedLogOnConfirmation = new ExtendedLogOnConfirmation();
            InteractionRequestedEventArgs request = new InteractionRequestedEventArgs(assignExtendedLogOnConfirmation, () => { });

            Application.Services.Interaction.InteractionRequest(request);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Verifies the operation access.
        /// </summary>
        /// <param name="operatorID">The operator ID.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="transactionId">The transaction id.</param>
        /// <returns>
        /// /// True if operator has access, false otherwise.
        /// </returns>
        private bool VerifyOperationAccess(string operatorID, PosisOperations operation, string transactionId)
        {
            bool result = true;
            IUserAccessSystem userAccess = Application.BusinessLogic.UserAccessSystem;

            if (!userAccess.UserHasAccess(operatorID, operation))
            {
                ManagerAccessConfirmation managerAccessInteraction = new ManagerAccessConfirmation() { Operation = (int)operation };

                // If a manager key is already in "Supervisor" position then don't prompt manager access.
                if (Application.Services.Peripherals.KeyLock.SupervisorPosition())
                {
                    managerAccessInteraction.Confirmed = true;
                }
                else
                {
                    InteractionRequestedEventArgs request = new InteractionRequestedEventArgs(managerAccessInteraction, () => { });
                    Application.Services.Interaction.InteractionRequest(request);
                }

                if (managerAccessInteraction.Confirmed)
                {
                    string authorizedBy = string.IsNullOrWhiteSpace(managerAccessInteraction.OperatorId) // If no operator ID is found then key was used
                        ? "Keylock"
                        : managerAccessInteraction.OperatorId;

                    // Log manager authorizations to audit log
                    ApplicationLog.WriteAuditEntry("LogOn:VerifyOperationAccess()",
                        string.Format("Manager '{0}' authorized the operation '{1}' for transaction '{2}'", authorizedBy, operation, transactionId));
                }
                else
                {
                    ApplicationLog.WriteAuditEntry("LogOn:VerifyOperationAccess()",
                        string.Format("Manager authorization either failed or was cancelled for operation '{0}'.", operation));

                    Application.Services.Dialog.ShowMessage(3540, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    result = false;
                }
            }

            return result;
        }

        #endregion

    }
}
