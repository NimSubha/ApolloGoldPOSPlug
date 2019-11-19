/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

namespace Microsoft.Dynamics.Retail.FiscalPrinter.RussianFiscalPrinter
{
    public sealed partial class RussianFiscalPrinter
    {
        #region Implementation of ICashDrawer

        /// <summary>
        /// Open the cash drawer.
        /// </summary>
        public void OpenDrawer()
        {
            FiscalPrinterDriverFactory.FiscalPrinterDriver.OpenDrawer();
        }

        /// <summary>
        /// Check if cash drawer is open.
        /// </summary>
        /// <returns>True if open, false otherwise.</returns>
        public bool DrawerOpen()
        {
            return false;
        }

        /// <summary>
        /// Check if the cash drawer is capable of reporting back whether it's closed or open.
        /// </summary>
        /// <returns>True if capable, false otherwise.</returns>
        public bool CapStatus()
        {
            return false;
        }

        #endregion
    }
}