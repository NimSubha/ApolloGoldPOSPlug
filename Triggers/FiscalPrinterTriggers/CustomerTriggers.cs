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
    [Export(typeof(ICustomerTrigger))]
    public sealed class CustomerTriggers : ICustomerTrigger
    {
        [Import]
        public IApplication Application { get; set; }

        #region ICustomerTriggersV1 Members

        public void PreCustomerClear(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.CustomerTriggers", "PreCustomerClear", LSRetailPosis.LogTraceLevel.Trace);

            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.PreCustomerClear(preTriggerResult, posTransaction);
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        public void PostCustomerClear(IPosTransaction posTransaction)
        {
            //
            //Left empty on purpose
            //
        }

        #endregion

        #region ICustomerTriggersV2 Members
        /// <summary>
        /// Triggered at the beginning
        /// </summary>
        /// <param name="preTriggerResult"></param>
        /// <param name="posTransaction"></param>
        public void PreCustomer(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.CustomerTriggers", "PreCustomer", LSRetailPosis.LogTraceLevel.Trace);
            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.PreCustomer(preTriggerResult, posTransaction);
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        /// <summary>
        /// Triggered at the end
        /// </summary>
        /// <param name="posTransaction"></param>
        public void PostCustomer(IPosTransaction posTransaction)
        {
            //
            //Left empty on purpose
            //
        }

        /// <summary>
        /// Triggered just before the customer is set
        /// </summary>
        /// <param name="preTriggerResult"></param>
        /// <param name="posTransaction"></param>
        /// <param name="customerId"></param>
        public void PreCustomerSet(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction, string customerId)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.CustomerTriggers", "PreCustomerSet", LSRetailPosis.LogTraceLevel.Trace);
            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.PreCustomerSet(preTriggerResult, posTransaction, customerId);
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        /// <summary>
        /// Triggered before customer search
        /// </summary>
        /// <param name="preTriggerResult"></param>
        /// <param name="posTransaction"></param>
        public void PreCustomerSearch(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.CustomerTriggers", "PreCustomerSearch", LSRetailPosis.LogTraceLevel.Trace);
            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.PreCustomerSearch(preTriggerResult, posTransaction);
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }        
        }

        /// <summary>
        /// Triggered after customer search
        /// </summary>
        /// <param name="posTransaction"></param>
        public void PostCustomerSearch(IPosTransaction posTransaction)
        {
            //
            //Left empty on purpose
            //
        }
        #endregion
    }
}
