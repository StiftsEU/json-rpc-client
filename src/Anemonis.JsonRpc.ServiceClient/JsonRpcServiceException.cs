// © Alexander Kozlenko. Licensed under the MIT License.

using System;

namespace Anemonis.JsonRpc.ServiceClient
{
    /// <summary>Represents an error that occurs during invocation of a JSON-RPC service method.</summary>
    public sealed class JsonRpcServiceException : JsonRpcException
    {
        internal JsonRpcServiceException()
            : base()
        {
        }

        internal JsonRpcServiceException(string message)
            : base(message)
        {
        }

        internal JsonRpcServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        internal JsonRpcServiceException(long code, string message, object errorData, bool hasErrorData)
            : base(message)
        {
            Code = code;
            ErrorData = errorData;
            HasErrorData = hasErrorData;
        }

        /// <summary>Gets a number that indicates the error type that occurred.</summary>
        public long Code
        {
            get;
        }

        /// <summary>Gets an optional value that contains additional information about the error.</summary>
        public object ErrorData
        {
            get;
        }

        /// <summary>Gets a value indicating whether the additional information about the error is specified.</summary>
        public bool HasErrorData
        {
            get;
        }
    }
}
