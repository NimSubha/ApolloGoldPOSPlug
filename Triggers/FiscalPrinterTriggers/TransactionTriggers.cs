/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.ComponentModel.Composition;
using System.Data.SqlClient;
using LSRetailPosis.Transaction;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Triggers;
using Microsoft.Dynamics.Retail.Pos.Contracts;

namespace Microsoft.Dynamics.Retail.FiscalPrinter.Triggers
{
    [Export(typeof(ITransactionTrigger))]
    public sealed class TransactionTriggers : ITransactionTrigger
    {
        [Import]
        public IApplication Application { get; set; }

        #region ITransactionTriggers Members

        public void BeginTransaction(IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.TransactionTriggers", "BeginTransaction", LSRetailPosis.LogTraceLevel.Trace);
            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.BeginTransaction(posTransaction);
                }
            }
            catch (FiscalPrinterException ex)
            {
                UserMessages.HandleException(ex);
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        public void SaveTransaction(IPosTransaction posTransaction, SqlTransaction sqlTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.TransactionTriggers", "SaveTransaction", LSRetailPosis.LogTraceLevel.Trace);
            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.SaveTransaction(posTransaction, sqlTransaction);
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        public void PreEndTransaction(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.TransactionTriggers", "PreEndTransaction", LSRetailPosis.LogTraceLevel.Trace);
            try
            {
                if (!(posTransaction is LogOnOffTransaction) 
                    && Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.PreEndTransaction(preTriggerResult, posTransaction);
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        public void PostEndTransaction(IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.TransactionTriggers", "PostEndTransaction", LSRetailPosis.LogTraceLevel.Trace);
            try
            {
                if (!(posTransaction is LogOnOffTransaction) 
                    && Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.PostEndTransaction(posTransaction);
                    return;
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        public void PreVoidTransaction(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.TransactionTriggers", "PreVoidTransaction", LSRetailPosis.LogTraceLevel.Trace);
            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.PreVoidTransaction(preTriggerResult, posTransaction);
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        public void PostVoidTransaction(IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.TransactionTriggers", "PostVoidTransaction", LSRetailPosis.LogTraceLevel.Trace);
            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.PostVoidTransaction(posTransaction);
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        public void PreReturnTransaction(IPreTriggerResult preTriggerResult, IRetailTransaction originalTransaction, IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.TransactionTriggers", "PreReturnTransaction", LSRetailPosis.LogTraceLevel.Trace);
            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.PreReturnTransaction(preTriggerResult, originalTransaction, posTransaction);
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        public void PostReturnTransaction(IPosTransaction posTransaction)
        {
            //
            //Left empty on purpose
            //
        }

        public void PreConfirmReturnTransaction(IPreTriggerResult preTriggerResult, IRetailTransaction originalTransaction)
        {
            //
            //Left empty on purpose
            //
        }

        public void PreRollbackTransaction(IPreTriggerResult preTriggerResult, IPosTransaction originalTransaction)
        {
            // Left empty on purpose.
        }

        #endregion
    }
}
