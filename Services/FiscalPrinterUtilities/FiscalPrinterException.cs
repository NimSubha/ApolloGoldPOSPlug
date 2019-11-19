/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Runtime.Serialization;
using LSRetailPosis;
using System.Security.Permissions;

namespace Microsoft.Dynamics.Retail.FiscalPrinter
{
    /// <summary>
    /// Class that extends PosisException to provide an specific Fiscal printer exception.
    /// </summary>
    [Serializable]
    public class FiscalPrinterException : PosisException
    {        
        /// <summary>
        /// FiscalPrinterException constructor.
        /// </summary>
        public FiscalPrinterException()
        { }

        /// <summary>
        /// FiscalPrinterException constructor.
        /// </summary>
        /// <param name="message"></param>
        public FiscalPrinterException(string message)
            : base(message)
        { }

        /// <summary>
        /// FiscalPrinterException constructor.
        /// </summary>
        /// <param name="message">The message describing the exception.</param>
        /// <param name="messageWasShownToTheUser">A Boolean determining whether the message was already shown to the user.</param>
        public FiscalPrinterException(string message, bool messageWasShownToTheUser) : base(message)
        {
            MessageWasShownToTheUser = messageWasShownToTheUser;
        }

        /// <summary>
        /// FiscalPrinterException constructor.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public FiscalPrinterException(string message, Exception innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// FiscalPrinterException constructor.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected FiscalPrinterException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            MessageWasShownToTheUser = info.GetBoolean("MessageWasShownToTheUser");
        }

        /// <summary>
        /// FiscalPrinterException constructor.
        /// </summary>
        /// <param name="errorNumber"></param>
        /// <param name="originalException"></param>
        public FiscalPrinterException(int errorNumber, Exception originalException)
            : base(Resources.Translate(errorNumber), originalException)
        { 
            ErrorNumber = errorNumber;
            ErrorMessageNumber = errorNumber;
        }

        /// <summary>
        /// FiscalPrinterException constructor.
        /// </summary>
        /// <param name="errorNumber"></param>
        public FiscalPrinterException(int errorNumber)
            : this(Resources.Translate(errorNumber))
        {
            ErrorNumber = errorNumber;
            ErrorMessageNumber = errorNumber;
        }

        /// <summary>
        /// FiscalPrinterException constructor.
        /// </summary>
        /// <param name="errorNumber"></param>
        /// <param name="args"></param>
        public FiscalPrinterException(int errorNumber, params object[] args)
            : this(Resources.Translate(errorNumber, args))
        {
            ErrorNumber = errorNumber;
            ErrorMessageNumber = errorNumber;
        }

        /// <summary>
        /// FiscalPrinterException constructor.
        /// </summary>
        /// <param name="errorNumber"></param>
        /// <param name="errorMessageNumber"></param>
        /// <param name="args"></param>
        public FiscalPrinterException(int errorNumber, int errorMessageNumber, params object[] args)
            : base(Resources.Translate(errorMessageNumber, args))
        {
            ErrorNumber = errorNumber;
            ErrorMessageNumber = errorMessageNumber;
        }

        /// <summary>
        /// Gets or sets a boolean value determining whether the error message was shown to the end user.
        /// </summary>
        public bool MessageWasShownToTheUser { get; private set; }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("MessageWasShownToTheUser", MessageWasShownToTheUser);
        }
    }
}
