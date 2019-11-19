/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Linq;
using System.Windows.Forms;
using LSRetailPosis.POSProcesses;
using Microsoft.Dynamics.Retail.PaymentSDK;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using Microsoft.Dynamics.Retail.SDKManager;

namespace Microsoft.Dynamics.Retail.Pos.EFTServiceOperationsService
{
    /// <summary>
    /// Base class for service EFT operations 
    /// </summary>
    internal abstract class EFTServiceOperationBase
    {

        private const int InvalidConnectorTypeMessageID = 105002;
        private const int ConnectorIsNotSetupMessageID = 3273;

        private IPosTransaction posTransaction;
        private IDialog dialog;
        private IPrinting printing;

        #region Constructor

        /// <summary>
        /// Initializes a <see cref="EFTServiceOperationBase"/> instance.
        /// </summary>
        /// <param name="posTransaction">POS transaction</param>
        /// <param name="dialog">Dialog service</param>
        /// <param name="printing">Printing service</param>
        public EFTServiceOperationBase(IPosTransaction posTransaction, IDialog dialog, IPrinting printing)
        {
            this.posTransaction = posTransaction;
            this.dialog = dialog;
            this.printing = printing;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Resource id for error message that the open transaction should be completed
        /// </summary>
        abstract protected int FinishCurrentTransactionErrorMessage { get; }

        /// <summary>
        /// Resource id for error message that the error occured during operation
        /// </summary>
        abstract protected int ConnectorErrorMessage { get; }

        /// <summary>
        /// Current operation namespace
        /// </summary>
        abstract protected string OperationNamespace { get; }

        /// <summary>
        /// Name of receipt property in the operation namespace
        /// <seealso cref="OperationNamespace"/>
        /// </summary>
        abstract protected string ExternalReceiptPropertyName  { get; }

        /// <summary>
        /// Get concrete operation from payment provider implementation
        /// </summary>
        /// <param name="eft"></param>
        /// <returns></returns>
        abstract protected Func<Request, Response> GetOperation(IEFTServiceOperations eft);

        #endregion

        #region Methods

        /// <summary>
        /// Determines if there are any errors in bank connector response
        /// </summary>
        /// <param name="response">Response</param>
        /// <returns>true if there are no errors in response; false otherwise.</returns>
        virtual protected bool VerifyResponse(Response response)
        {
            bool hasErrors = response != null && response.Errors != null && response.Errors.Length > 0;
            if (hasErrors)
            {
                string message = LSRetailPosis.ApplicationLocalizer.Language.Translate(ConnectorErrorMessage, response.Errors[0].Message);
                ShowErrorMessage(dialog, message);
            }
            return !hasErrors;
        }

        /// <summary>
        /// Verifies whether transaction is valid for the operation.
        /// </summary>
        /// <param name="posTransaction"></param>
        /// <returns>true if transaction is of valid type; false otherwise.</returns>
        virtual protected bool VerifyTransaction()
        {
            bool result = true;
            if (posTransaction is ISalesOrderTransaction ||
                posTransaction is ISalesInvoiceTransaction ||
                posTransaction is IRetailTransaction ||
                posTransaction is ICustomerPaymentTransaction)
            {
                POSFormsManager.ShowPOSMessageDialog(FinishCurrentTransactionErrorMessage);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Extracts receipt text form connector return
        /// </summary>
        /// <param name="response">Response from connector</param>
        /// <returns>Bank receipt</returns>
        protected string GetReceiptText(Response response)
        {
            string receiptText = string.Empty;
            if (response != null && response.Properties != null)
            {
                var property = response.Properties.FirstOrDefault(p => string.Equals(p.Namespace, OperationNamespace, StringComparison.Ordinal) &&
                                                                       string.Equals(p.Name, ExternalReceiptPropertyName, StringComparison.Ordinal));
                if (property != null && property.ValueType == DataType.String)
                {
                    receiptText = property.StringValue;
                }
            }
            return receiptText;
        }

        /// <summary>
        /// Prints receipt on fiscal printer
        /// </summary>
        /// <param name="printer">Printer interface</param>
        /// <param name="receiptText">Text to print</param>
        protected void PrintReceipt(string receiptText)
        {
            if (!string.IsNullOrWhiteSpace(receiptText))
            {
                printing.PrintExternalReceipt(receiptText);
            }
        }

        /// <summary>
        /// Run operation
        /// </summary>
        /// <exception cref="EFTServiceOperationException">Thrown, if EFT operation failed.</exception>
        public void RunOperation()
        {
            bool success = false;

            if (VerifyTransaction())
            {
                var eftConnector = GetEFTServiceOperationsConnector();
                if (eftConnector != null)
                {
                    var request = new Request();
                    request.Locale = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
                    var eftOperation = GetOperation(eftConnector);
                    var response = eftOperation(request);

                    if (VerifyResponse(response))
                    {
                        success = true;

                        string receiptText = GetReceiptText(response);
                        PrintReceipt(receiptText);
                    }
                }
            }

            if (!success)
            {
                throw new LSRetailPosis.EFTServiceOperationException("EFT service operation failed.", null);
            }
        }

        /// <summary>
        /// Gets EFT service operations connector
        /// </summary>
        /// <returns>Connector</returns>
        public static IEFTServiceOperations GetEFTServiceOperationsConnector()
        {
            string connectorName = LSRetailPosis.Settings.HardwareProfiles.EFT.EftPaymentConnectorName;
            if (string.IsNullOrWhiteSpace(connectorName))
            {
                POSFormsManager.ShowPOSMessageDialog(ConnectorIsNotSetupMessageID);
                return null;
            }

            var processor = PaymentProcessorManager.GetPaymentProcessor(connectorName);
            if (!((PaymentProcessorWrapper)processor).IsInterfaceSupportedByConnector<IEFTServiceOperations>())
            {
                POSFormsManager.ShowPOSMessageDialog(InvalidConnectorTypeMessageID);
                return null;
            }
            var eftServiceOperations = processor as IEFTServiceOperations;

            return eftServiceOperations;
        }

        /// <summary>
        /// Determines is EFT service operations supported
        /// </summary>
        /// <returns>True if EFT service operations is supported, otherwise false</returns>
        public static bool IsEFTServiceOperationsSupported()
        {
            string connectorName = LSRetailPosis.Settings.HardwareProfiles.EFT.EftPaymentConnectorName;
            if (string.IsNullOrWhiteSpace(connectorName))
            {
                return false;
            }

            var processor = PaymentProcessorManager.GetPaymentProcessor(connectorName);
            if (!((PaymentProcessorWrapper)processor).IsInterfaceSupportedByConnector<IEFTServiceOperations>())
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Shows error message
        /// </summary>
        /// <param name="dialog">Dialog</param>
        /// <param name="message">Message</param>
        protected static void ShowErrorMessage(IDialog dialog, string message)
        {
            dialog.ShowMessage(message, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion
    }
}
