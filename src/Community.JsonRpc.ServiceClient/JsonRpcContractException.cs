// © Alexander Kozlenko. Licensed under the MIT License.

using System;

namespace Community.JsonRpc.ServiceClient
{
    /// <summary>Represents an error that occur during service result handling.</summary>
    public sealed class JsonRpcContractException : Exception
    {
        internal JsonRpcContractException(string requestId, string message)
            : base(message)
        {
            RequestId = requestId;
        }

        internal JsonRpcContractException(string requestId, string message, Exception inner)
            : base(message, inner)
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