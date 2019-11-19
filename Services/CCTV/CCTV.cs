/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;

namespace Microsoft.Dynamics.Retail.Pos.CCTV
{
    // According to Chriag this class is loaded by reflection (see PS#3187)
    [Export(typeof(ICCTV))]
    class CCTV : ICCTV
    {
        // Get all text through the Translation function in the ApplicationLocalizer
        // TextID's for the CCTV service are reserved at 58000 - 58999

        /// <summary>
        /// Gives CCTV output details as per passed parameters.
        /// </summary>
        /// <param name="posTransaction"></param>
        /// <param name="operationId"></param>
        /// <param name="mainOperation"></param>
        /// <param name="operationInfo"></param>
        /// <param name="text"></param>
        public void CCTVOutput(IPosTransaction posTransaction, PosisOperations operationId, bool mainOperation, object operationInfo, string text)
        { 
            LSRetailPosis.POSProcesses.OperationInfo opInfo = (LSRetailPosis.POSProcesses.OperationInfo)operationInfo;
        }

        public void CCTVMessageOutput(string text)
        {
        }

        public void CCTVErrorOutput(string text)
        {
        }
    }
}
