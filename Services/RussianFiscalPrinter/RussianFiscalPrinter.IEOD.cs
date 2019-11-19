/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using System;

namespace Microsoft.Dynamics.Retail.FiscalPrinter.RussianFiscalPrinter
{
    public sealed partial class RussianFiscalPrinter 
    {
        #region Implementation of IEOD

        /// <summary>
        /// Print Report for currently opend batch (X-Report)
        /// </summary>
        /// <param name="transaction"></param>
        public void PrintXReport(IPosTransaction transaction)
        {
            try
            {
                FiscalPrinterDriverFactory.FiscalPrinterDriver.PrintXReport();
            }
            catch (FiscalPrinterException)
            {
                // We do not want to handle FiscalPrinterException here, we want to enforce standard handling defined up the call stack.
                throw;
            }
            catch (Exception ex)
            {
                ExceptionHelper.ThrowException(Resources.ErrorWhilePrinting, ex.Message);
            }
        }

        /// <summary>
        /// Print recently closed batch report (Z-Report)
        /// </summary>
        /// <param name="transaction"></param>.
        public void PrintZReport(IPosTransaction transaction)
        {
            if (transaction != null)
            {
                try
                {
                    FiscalPrinterDriverFactory.FiscalPrinterDriver.GenerateZReportAndFiles(Properties.Settings.Default.ZReportFilePath);
                }
                catch (FiscalPrinterException)
                {
                    // We do not want to handle FiscalPrinterException here, we want to enforce standard handling defined up the call stack.
                    throw;
                }
                catch (Exception ex)
                {
                    ExceptionHelper.ThrowException(Resources.ErrorWhilePrinting, ex.Message);
                }
            }
        }

        #endregion        
    }
}
