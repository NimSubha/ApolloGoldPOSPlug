/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Triggers;
using Microsoft.Dynamics.Retail.Pos.Contracts;

namespace Microsoft.Dynamics.Retail.FiscalPrinter.Triggers
{

    [Export(typeof(IDiscountTrigger))]
    public sealed class DiscountTriggers : IDiscountTrigger
    {
        [Import]
        public IApplication Application { get; set; }

        #region IDiscountTriggersV1 Members

        public void PreLineDiscountAmount(IPreTriggerResult preTriggerResult, IPosTransaction transaction, int LineId)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.DiscountTriggers", "PreLineDiscountAmount", LSRetailPosis.LogTraceLevel.Trace);
            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.PreLineDiscountAmount(preTriggerResult, transaction, LineId);
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }           
        }

        public void PreLineDiscountPercent(IPreTriggerResult preTriggerResult, IPosTransaction transaction, int LineId)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.DiscountTriggers", "PreLineDiscountPercent", LSRetailPosis.LogTraceLevel.Trace);
            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.PreLineDiscountPercent(preTriggerResult, transaction, LineId);
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        #endregion

        #region IDiscountTriggersV2 Members

        public void PostLineDiscountAmount(IPosTransaction posTransaction, int lineId)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.DiscountTriggers", "PostLineDiscountAmount", LSRetailPosis.LogTraceLevel.Trace);
            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.PostLineDiscountAmount(posTransaction, lineId);
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        public void PostLineDiscountPercent(IPosTransaction posTransaction, int lineId)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.DiscountTriggers", "PostLineDiscountPercent", LSRetailPosis.LogTraceLevel.Trace);
            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.PostLineDiscountPercent(posTransaction, lineId);
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        public void PreTotalDiscountAmount(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.DiscountTriggers", "PreTotalDiscountAmount", LSRetailPosis.LogTraceLevel.Trace);
            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.PreTotalDiscountAmount(preTriggerResult, posTransaction);
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        public void PostTotalDiscountAmount(IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.DiscountTriggers", "PostTotalDiscountAmount", LSRetailPosis.LogTraceLevel.Trace);
            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.PostTotalDiscountAmount(posTransaction);
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        public void PreTotalDiscountPercent(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.DiscountTriggers", "PreTotalDiscountPercent", LSRetailPosis.LogTraceLevel.Trace);
            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.PreTotalDiscountPercent(preTriggerResult, posTransaction);
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        public void PostTotalDiscountPercent(IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.DiscountTriggers", "PostTotalDiscountPercent", LSRetailPosis.LogTraceLevel.Trace);
            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.PostTotalDiscountPercent(posTransaction);
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
