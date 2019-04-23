// © Alexander Kozlenko. Licensed under the MIT License.

using System;

namespace Anemonis.JsonRpc.ServiceClient
{
    /// <summary>Represents an error that occurs during processing JSON-RPC method parameters, result, or error data.</summary>
    public sealed class JsonRpcClientException : JsonRpcException
    {
        private readonly JsonRpcId _requestId;

        internal JsonRpcClientException(string message)
            : base(message)
        {
        }

        internal JsonRpcClientException(string message, in JsonRpcId requestId)
            : base(message)
        {
            _requestId = requestId;
        }

        internal JsonRpcClientException(string message, in JsonRpcId requestId, Exception innerException)
            : base(message, innerException)
        {
            _requestId = requestId;
        }

        /// <summary>Gets the identifier of the related JSON-RPC request.</summary>
        public ref readonly JsonRpcId RequestId
        {
            get => ref _requestId;
        }
    }
}
