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
    [Export(typeof(IPaymentTrigger))]
    public sealed class PaymentTriggers : IPaymentTrigger
    {
        [Import]
        public IApplication Application { get; set; }

        #region IPaymentTriggers Members

        public void PrePayCustomerAccount(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction, decimal amount)
        {
            //
            //Left empty on purpose
            //
        }

        public void PrePayCardAuthorization(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction, ICardInfo cardInfo, decimal amount)
        {
            //
            //Left empty on purpose
            //
        }

        public void OnPayment(IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.PaymentTriggers", "OnPayment", LSRetailPosis.LogTraceLevel.Trace);
            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.OnPayment(posTransaction);
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        public void PrePayment(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction, object posOperation, string tenderId)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.PaymentTriggers", "PrePayment", LSRetailPosis.LogTraceLevel.Trace);
            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.PrePayment(preTriggerResult, posTransaction, posOperation, tenderId);
                    return;
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        /// <summary>
        /// Triggered before voiding of a payment.
        /// </summary>
        /// <param name="preTriggerResult"></param>
        /// <param name="posTransaction"></param>
        /// <param name="lineId"> </param>
        public void PreVoidPayment(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction, int lineId)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.PaymentTriggers", "PreVoidPayment", LSRetailPosis.LogTraceLevel.Trace);
            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.PreVoidPayment(preTriggerResult, posTransaction, lineId);
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        /// <summary>
        /// Triggered after voiding of a payment.
        /// </summary>
        /// <param name="posTransaction"></param>
        /// <param name="lineId"> </param>
        public void PostVoidPayment(IPosTransaction posTransaction, int lineId)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.PaymentTriggers", "PostVoidPayment", LSRetailPosis.LogTraceLevel.Trace);
            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.PostVoidPayment(posTransaction, lineId);
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        /// <summary>
        /// Triggered before registering cash payment.
        /// </summary>
        /// <param name="preTriggerResult"></param>
        /// <param name="posTransaction"></param>
        /// <param name="posOperation"></param>
        /// <param name="tenderId"></param>
        /// <param name="currencyCode"></param>
        /// <param name="amount"></param>
        public void PreRegisterPayment(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction, object posOperation, string tenderId, string currencyCode, decimal amount)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.PaymentTriggers", "PreRegisterPayment", LSRetailPosis.LogTraceLevel.Trace);
        }

        #endregion
    }
}
