/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using Microsoft.Dynamics.Retail.PaymentSDK;
using Microsoft.Dynamics.Retail.PaymentSDK.Constants;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;

namespace Microsoft.Dynamics.Retail.Pos.EFTServiceOperationsService
{
    internal class EFTServiceOperationLastReceipt : EFTServiceOperationBase
    {

        public EFTServiceOperationLastReceipt(IPosTransaction posTransaction, IDialog dialog, IPrinting printing)
            : base(posTransaction, dialog, printing)
        {
        }

        protected override int FinishCurrentTransactionErrorMessage
        {
            get
            {
                return 105007; //Finish the current transaction before reprint bank cheque.
            }
        }

        protected override int ConnectorErrorMessage
        {
            get
            {
                return 105008; //An error ocurred performing reprint bank cheque. Error message: {0}
            }
        }

        protected override string ExternalReceiptPropertyName
        {
            get
            {
                return XReportProperties.ExternalReceipt;
            }
        }

        protected override string OperationNamespace
        {
            get
            {
                return GenericNamespace.LastReceipt;
            }
        }

        protected override Func<Request, Response> GetOperation(IEFTServiceOperations eft)
        {
            return eft.GetLastReceipt;
        }
    }
}
