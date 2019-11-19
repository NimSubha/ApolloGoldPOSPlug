/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Triggers;

namespace Microsoft.Dynamics.Retail.FiscalPrinter.Triggers
{
    [Export(typeof(IOperationTrigger))]
    public sealed class OperationTriggers : IOperationTrigger
    {
        [Import]
        public IApplication Application { get; set; }

        #region IOperationTriggersV1 Members

        /// <summary>
        /// Before the operation is processed this trigger is called.
        /// </summary>
        /// <param name="preTriggerResult"></param>
        /// <param name="posTransaction"></param>
        /// <param name="posisOperation"></param>
        public void PreProcessOperation(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction, PosisOperations posisOperation)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.OperationTriggers", "PreProcessOperation", LSRetailPosis.LogTraceLevel.Trace);

            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.PreProcessOperation(preTriggerResult, posTransaction, posisOperation);
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }

        }

        /// <summary>
        /// After the operation has been processed this trigger is called.
        /// </summary>
        /// <param name="posTransaction"></param>
        /// <param name="posisOperation"></param>
        public void PostProcessOperation(IPosTransaction posTransaction, PosisOperations posisOperation)
        {
            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.PostProcessOperation(posTransaction, posisOperation);
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        #endregion
    }
}
