// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.Data.JsonRpc;
using System.Net;

namespace Community.JsonRpc.ServiceClient
{
    /// <summary>Represents an error that occurs during communication with a JSON-RPC service.</summary>
    public sealed class JsonRpcProtocolException : JsonRpcException
    {
        private readonly HttpStatusCode _httpStatusCode;
        private readonly JsonRpcId _requestId;

        internal JsonRpcProtocolException(HttpStatusCode httpStatusCode, string message)
            : base(message)
        {
            _httpStatusCode = httpStatusCode;
        }

        internal JsonRpcProtocolException(HttpStatusCode httpStatusCode, string message, in JsonRpcId requestId)
            : base(message)
        {
            _httpStatusCode = httpStatusCode;
            _requestId = requestId;
        }

        internal JsonRpcProtocolException(HttpStatusCode httpStatusCode, string message, in JsonRpcId requestId, Exception innerException)
            : base(message, innerException)
        {
            _httpStatusCode = httpStatusCode;
            _requestId = requestId;
        }

        /// <summary>Gets the status code of the HTTP response.</summary>
        public HttpStatusCode HttpStatusCode
        {
            get => _httpStatusCode;
        }

        /// <summary>Gets the identifier of the related JSON-RPC request.</summary>
        public ref readonly JsonRpcId RequestId
        {
            get => ref _requestId;
        }
    }
}