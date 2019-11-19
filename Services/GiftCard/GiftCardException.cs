/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Runtime.Serialization;

namespace Microsoft.Dynamics.Retail.Pos.GiftCard
{
	/// <summary>The exception that is thrown when something goes wrong in the secure remoting channel.</summary>
	[Serializable]
	public class GiftCardException : Exception
	{
		/// <summary>Initializes a new instance of the GiftCardException class with default properties.</summary>
		public GiftCardException()
		{
		}

		/// <summary>Initializes a new instance of the GiftCardException class with the given message.</summary>
		/// <param name="message">The error message that explains why the exception occurred.</param>
		public GiftCardException(string message) :
			base(message)
		{
		}

		/// <summary>Initializes a new instance of the GiftCardException class with the specified properties.</summary>
		/// <param name="message">The error message that explains why the exception occurred.</param>
		/// <param name="innerException">The exception that is the cause of the current exception.</param>
		public GiftCardException(string message, System.Exception innerException) :
			base(message, innerException)
		{
		}

		/// <summary>Initializes the exception with serialized information.</summary>
		/// <param name="info">Serialization information.</param>
		/// <param name="context">Streaming context.</param>
		protected GiftCardException(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
		}
	}
}
