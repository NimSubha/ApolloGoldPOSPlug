/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using LSRetailPosis;

namespace Microsoft.Dynamics.Retail.FiscalPrinter
{
    public static class ExceptionHelper
    {
        /// <summary>
        /// Show the exception message to the user, logs it and raises the fiscal printer exception
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="args"></param>
        public static void ThrowException(int resourceId, params object[] args)
        {
            ThrowException(true, resourceId, args);
        }

        /// <summary>
        /// Show the exception message to the user, logs it and raises the fiscal printer exception
        /// </summary>
        /// <param name="promptUser"></param>
        /// <param name="resourceId"></param>
        /// <param name="args"></param>
        public static void ThrowException(bool promptUser, int resourceId, params object[] args)
        {
            var message = Resources.Translate(resourceId, args);

            if (promptUser)
            {
                //Make sure the user can click on OK
                SafeNativeMethodsHelper.BlockMouseAndKeyboard(false);
                UserMessages.ShowException(message);
            }
            LogHelper.LogError("RaiseException", message);
            throw new FiscalPrinterException(resourceId, args);
        }

        /// <summary>
        /// Show the exception message to the user, logs it and raises the fiscal printer exception
        /// </summary>
        /// <param name="errorNumber"> </param>
        /// <param name="resourceId"></param>
        /// <param name="args"></param>
        public static void ThrowException(int errorNumber, int resourceId, params object[] args)
        {
            ThrowException(true, errorNumber, resourceId, args);
        }

        /// <summary>
        /// Show the exception message to the user, logs it and raises the fiscal printer exception
        /// </summary>
        /// <param name="promptUser"></param>
        /// <param name="errorNumber"> </param>
        /// <param name="resourceId"></param>
        /// <param name="args"></param>
        public static void ThrowException(bool promptUser, int errorNumber, int resourceId, params object[] args)
        {
            var message = Resources.Translate(resourceId, args);

            if (promptUser)
            {
                //Make sure the user can click on OK
                SafeNativeMethodsHelper.BlockMouseAndKeyboard(false);
                UserMessages.ShowException(message);
            }
            LogHelper.LogError("RaiseException", message);
            throw new FiscalPrinterException(errorNumber, resourceId, args);
        }

        /// <summary>
        /// Show the exception message to the user, logs it and raises the fiscal printer exception
        /// </summary>
        public static void ThrowException(string message)
        {
            ThrowException(true, message);
        }

        /// <summary>
        /// Show the exception message to the user, logs it and raises the fiscal printer exception
        /// </summary>
        public static void ThrowException(bool promptUser, string message)
        {
            if (promptUser)
            {
                //Make sure the user can click on OK
                SafeNativeMethodsHelper.BlockMouseAndKeyboard(false);
                UserMessages.ShowException(message);
            }
            LogHelper.LogError("RaiseException", message);
            throw new FiscalPrinterException(message);
        }

        /// <summary>
        /// Show the exception message to the user, logs it and raises the PosStartupException
        /// </summary>
        public static void ThrowPosStartupException(int resourceId, params object[] args)
        {
            ThrowPosStartupException(true, resourceId, args);
        }

        /// <summary>
        /// Show the exception message to the user, logs it and raises the PosStartupException
        /// </summary>
        public static void ThrowPosStartupException(bool promptUser, int resourceId, params object[] args)
        {
            var message = Resources.Translate(resourceId, args);

            if (promptUser)
            {
                //
                //make sure the user can click on OK
                //
                SafeNativeMethodsHelper.BlockMouseAndKeyboard(false);
                UserMessages.ShowException(message);
            }
            LogHelper.LogError("RaiseException", message);
            throw new PosStartupException(message);
        }
    }
}
