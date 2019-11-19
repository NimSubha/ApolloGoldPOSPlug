/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System.Windows.Forms;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;

namespace Microsoft.Dynamics.Retail.Pos.EFTServiceOperationsService
{
    /// <summary>
    /// Class implementing the interface IEFTServiceOperationsService
    /// </summary>
    [Export(typeof(IEFTServiceOperationsService))]
    public class EFTServiceOperationsService : IEFTServiceOperationsService
    {
        #region IEFTServiceOperationsService Members

        /// <summary>
        /// Run totals verification procedure
        /// </summary>
        /// <param name="posTransaction">Transaction</param>
        public void RunTotalsVerification(IPosTransaction posTransaction)
        {
            var serviceOperation = new EFTServiceOperationTotalVerification(posTransaction,
                                        Application.Services.Dialog, Application.Services.Printing);
            serviceOperation.RunOperation();
        }

        /// <summary>
        /// Run bank X Report
        /// </summary>
        /// <param name="posTransaction">Transaction</param>
        public void RunXReport(IPosTransaction posTransaction)
        {
            var serviceOperation = new EFTServiceOperationXReport(posTransaction,
                                        Application.Services.Dialog, Application.Services.Printing);
            serviceOperation.RunOperation();
        }

        /// <summary>
        /// Get bank cards last operation receipt
        /// </summary>
        /// <param name="posTransaction">Transaction</param>
        public void ProcessLastReceipt(IPosTransaction posTransaction)
        {
            var serviceOperation = new EFTServiceOperationLastReceipt(posTransaction,
                                        Application.Services.Dialog, Application.Services.Printing);
            serviceOperation.RunOperation();
        }

        /// <summary>
        /// Determines is EFT service operations supported
        /// </summary>
        /// <returns>True if EFT service operations is supported, otherwise false</returns>
        public bool IsSupported()
        {
            return EFTServiceOperationBase.IsEFTServiceOperationsSupported();
        }

        #endregion

        /// <summary>
        /// Gets or sets the IApplication instance.
        /// </summary>
        [Import]
	    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Automatic property implementation requires a set method.")]
        protected IApplication Application { get; private set; }
    }
}
