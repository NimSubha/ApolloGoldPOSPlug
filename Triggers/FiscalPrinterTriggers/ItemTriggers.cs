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
    [Export(typeof(IItemTrigger))]
    public sealed class ItemTriggers : IItemTrigger
    {
        [Import]
        public IApplication Application { get; set; }

        #region IItemTriggersV1 Members

        public void PreSale(IPreTriggerResult preTriggerResult, ISaleLineItem saleLineItem, IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.ItemTriggers", "PreSale", LSRetailPosis.LogTraceLevel.Trace);
            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.PreSale(preTriggerResult, saleLineItem, posTransaction);
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        public void PostSale(IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.ItemTriggers", "PostSale", LSRetailPosis.LogTraceLevel.Trace);
            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.PostSale(posTransaction);
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        public void PreReturnItem(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.ItemTriggers", "PreReturnItem", LSRetailPosis.LogTraceLevel.Trace);
            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.PreReturnItem(preTriggerResult, posTransaction);
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            } 
        }

        public void PostReturnItem(IPosTransaction posTransaction)
        {
            //
            //Left empty on purpose
            //
        }

        public void PreVoidItem(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction, int lineId)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.ItemTriggers", "PreVoidItem", LSRetailPosis.LogTraceLevel.Trace);
            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.PreVoidItem(preTriggerResult, posTransaction, lineId);
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            } 
        }

        public void PostVoidItem(IPosTransaction posTransaction, int lineId)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.ItemTriggers", "PostVoidItem", LSRetailPosis.LogTraceLevel.Trace);
            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.PostVoidItem(posTransaction, lineId);
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            } 
        }

        public void PreSetQty(IPreTriggerResult preTriggerResult, ISaleLineItem saleLineItem, IPosTransaction posTransaction, int lineId)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.ItemTriggers", "PreSetQty", LSRetailPosis.LogTraceLevel.Trace);
            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.PreSetQty(preTriggerResult, saleLineItem, posTransaction, lineId);
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        public void PostSetQty(IPosTransaction posTransaction, ISaleLineItem saleLineItem)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.ItemTriggers", "PostSetQty", LSRetailPosis.LogTraceLevel.Trace);
            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.PostSetQty(posTransaction, saleLineItem);
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        public void PrePriceOverride(IPreTriggerResult preTriggerResult, ISaleLineItem saleLineItem, IPosTransaction posTransaction, int lineId)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.ItemTriggers", "PrePriceOverride", LSRetailPosis.LogTraceLevel.Trace);
            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.PrePriceOverride(preTriggerResult, saleLineItem, posTransaction, lineId);
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        public void  PostPriceOverride(IPosTransaction posTransaction, ISaleLineItem saleLineItem)
        {
            //
            //Left empty on purpose
            //
        }

        #endregion

        #region IItemTriggersV2 Members

        public void PreClearQty(IPreTriggerResult preTriggerResult, ISaleLineItem saleLineItem, IPosTransaction posTransaction, int lineId)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.ItemTriggers", "PreClearQty", LSRetailPosis.LogTraceLevel.Trace);
            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.PreClearQty(preTriggerResult, saleLineItem, posTransaction, lineId);
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        public void PostClearQty(IPosTransaction posTransaction, ISaleLineItem saleLineItem)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.ItemTriggers", "PostClearQty", LSRetailPosis.LogTraceLevel.Trace);
            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.PostClearQty(posTransaction, saleLineItem);
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
