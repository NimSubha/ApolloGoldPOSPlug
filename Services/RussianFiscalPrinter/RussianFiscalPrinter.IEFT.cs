/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;

namespace Microsoft.Dynamics.Retail.FiscalPrinter.RussianFiscalPrinter
{
    public sealed partial class RussianFiscalPrinter
    {
        /// <summary>
        /// Verifies if there is EFT service integrated
        /// </summary>
        public bool IsThirdPartyCardPaymentEnabled()
        {
            return false;
        }

        /// <summary>
        /// Processes the card payment for the EFT provider.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="transaction"></param>
        public void ProcessCardPayment(IEFTInfo info, IRetailTransaction transaction)
        {
            // We do not support third-party EFT processors.
        }
    }
}
