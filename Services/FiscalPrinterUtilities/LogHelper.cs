/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using Microsoft.Dynamics.Retail.Diagnostics;
using System;

namespace Microsoft.Dynamics.Retail.FiscalPrinter
{
    public static class LogHelper
    {
        private const string LogPrefix = "FiscalPrinter.";

        /// <summary>
        /// Log an trace message in the POS logging destination
        /// </summary>
        /// <param name="source"></param>
        /// <param name="traceMessage"></param>
        /// <param name="args"></param>
        public static void LogTrace(string source, string traceMessage, params object[] args)
        {
            NetTracer.Information(String.Format("{0}{1} {2}", LogPrefix, source, String.Format(traceMessage, args)));
        }

        /// <summary>
        /// Log an debug message in the POS logging destination
        /// </summary>
        /// <param name="source"></param>
        /// <param name="debugMessage"></param>
        /// <param name="args"></param>
        public static void LogDebug(string source, string debugMessage, params object[] args)
        {
            NetTracer.Information(String.Format("{0}{1} {2}", LogPrefix, source, String.Format(debugMessage, args)));
        }

        /// <summary>
        /// Log an debug message in the POS logging destination
        /// </summary>
        /// <param name="source"></param>
        /// <param name="warningMessage"> </param>
        /// <param name="args"></param>
        public static void LogWarning(string source, string warningMessage, params object[] args)
        {
            NetTracer.Warning(String.Format("{0}{1} {2}", LogPrefix, source, String.Format(warningMessage, args)));
        }

        /// <summary>
        /// Log an error message in the POS logging destination
        /// </summary>
        /// <param name="source"></param>
        /// <param name="errorMessage"></param>
        /// <param name="args"></param>
        public static void LogError(string source, string errorMessage, params object[] args)
        {
            NetTracer.Error(String.Format("{0}{1} {2}", LogPrefix, source, String.Format(errorMessage, args)));
        }
    }
}
