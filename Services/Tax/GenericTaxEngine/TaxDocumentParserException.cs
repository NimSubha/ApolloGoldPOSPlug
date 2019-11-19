/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using System;
using System.Runtime.Serialization;

namespace Microsoft.Dynamics.Retail.Pos.Tax.GenericTaxEngine
{
    /// <summary>The exception that is thrown when something goes wrong in the tax document parsing.</summary>
    [Serializable]
    public class TaxDocumentParserException : Exception
    {
        /// <summary>Initializes a new instance of the TaxDocumentParserException class with default properties.</summary>
        public TaxDocumentParserException()
        {
        }

        /// <summary>Initializes a new instance of the TaxDocumentParserException class with the specified properties.</summary>
        /// <param name="errorCode">The error code that explains why the exception occurred.</param>
        public TaxDocumentParserException(int errorCode) :
            base(LSRetailPosis.ApplicationLocalizer.Language.Translate(errorCode))
        {
        }

        /// <summary>Initializes a new instance of the TaxDocumentParserException class with the specified properties.</summary>
        /// <param name="errorCode">The error code that explains why the exception occurred.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public TaxDocumentParserException(int errorCode, System.Exception innerException) :
            base(LSRetailPosis.ApplicationLocalizer.Language.Translate(errorCode), innerException)
        {
        }

        /// <summary>Initializes a new instance of the TaxDocumentParserException class with the given message.</summary>
        /// <param name="message">The error message that explains why the exception occurred.</param>
        public TaxDocumentParserException(string message) :
            base(message)
        {
        }

        /// <summary>Initializes a new instance of the TaxDocumentParserException class with the specified properties.</summary>
        /// <param name="message">The error message that explains why the exception occurred.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public TaxDocumentParserException(string message, System.Exception innerException) :
            base(message, innerException)
        {
        }

        /// <summary>Initializes the exception with serialized information.</summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Streaming context.</param>
        protected TaxDocumentParserException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }
    }
}
