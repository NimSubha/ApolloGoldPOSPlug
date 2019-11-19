/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts.Triggers;
using Microsoft.Dynamics.Retail.Pos.Contracts;

namespace Microsoft.Dynamics.Retail.FiscalPrinter.Triggers
{
    [Export(typeof(IApplicationTrigger))]
    public sealed class ApplicationTriggers : IApplicationTrigger
    {
        [Import]
        public IApplication Application { get; set; }

        #region IApplicationTriggers Members

        public void ApplicationStart()
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.ApplicationTriggers", "ApplicationStart", LSRetailPosis.LogTraceLevel.Trace);
            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.ApplicationStart();
                    return;
                }
            }
            catch (FiscalPrinterException e)
            {
                UserMessages.HandleException(e);
                return;
            }
        }

        public void ApplicationStop()
        {
            //
            //Left empty on purpose
            //
        }

        public void PostLogon(bool loginSuccessful, string operatorId, string name)
        {
            LSRetailPosis.ApplicationLog.Log("FiscalPrinter.ApplicationTriggers", "PostLogon", LSRetailPosis.LogTraceLevel.Trace);
            try
            {
                if (Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    Application.Services.Peripherals.FiscalPrinter.PostLogOn(loginSuccessful, operatorId, name);
                }
            }
            catch (Exception ex)
            {
                UserMessages.ShowException(ex.Message);
            }
        }

        public void PreLogon(IPreTriggerResult preTriggerResult, string operatorId, string name)
        {
            //
            //Left empty on purpose
            //
        }

        public void Logoff(string operatorId, string name)
        {
            //
            //Left empty on purpose
            //
        }

        public void LoginWindowVisible()
        {
            //
            //Left empty on purpose
            //
        }

        #endregion

    }
}
