// © Alexander Kozlenko. Licensed under the MIT License.

using System;

namespace Community.JsonRpc.ServiceClient
{
    /// <summary>Represents an error that occur during service result handling.</summary>
    public sealed class JsonRpcContractException : Exception
    {
        internal JsonRpcContractException(string message)
            : base(message)
        {
        }

        internal JsonRpcContractException(string message, string requestId)
            : base(message)
        {
            RequestId = requestId;
        }

        internal JsonRpcContractException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        internal JsonRpcContractException(string message, Exception innerException, string requestId)
            : base(message, innerException)
        {
            RequestId = requestId;
        }

        /// <summary>Gets the request identifier.</summary>
        public string RequestId
        {
            get;
        }
    }
}