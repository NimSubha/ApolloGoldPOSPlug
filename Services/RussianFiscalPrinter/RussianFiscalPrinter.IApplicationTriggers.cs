/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using Microsoft.Dynamics.Retail.Pos.SystemCore;

namespace Microsoft.Dynamics.Retail.FiscalPrinter.RussianFiscalPrinter
{
    public sealed partial class RussianFiscalPrinter
    {
        #region IApplicationTriggers implementation

        /// <summary>
        /// Triggers once, whenever the application starts.
        /// </summary>
        public void ApplicationStart()
        {
            //
            //Left empty on purpose
            //
        }

        /// <summary>
        /// Triggers after the login operation has been executed.
        /// </summary>
        public void PostLogOn(bool logOnSuccessful, string operatorId, string name)
        {
            LogHelper.LogTrace("FiscalPrinter.IApplicationTriggers.PostLogOn", "logOnSuccessful = {0}, operatorId = {1}, name = {2}", logOnSuccessful, operatorId, name);

            if (PosApplication.Instance.Shift != null)
            {
                if (RussianFiscalPrinterDriver.FiscalPrinterDriver.IsDayOpenedMoreThan24Hours())
                {
                    UserMessages.ShowWarning(Resources.PrinterDayIsOpenMoreThan24Hours);
                }

                if (!RussianFiscalPrinterDriver.FiscalPrinterDriver.IsDayOpened())
                {
                    RussianFiscalPrinterDriver.FiscalPrinterDriver.StartOfDay();

                    LogHelper.LogTrace("FiscalPrinter.IApplicationTriggers.PostLogOn", "new fiscal day started on printer");
                }
            }
        }

        #endregion
    }
}
