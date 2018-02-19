using System.Net;
using System.Net.Http;

namespace Community.JsonRpc.ServiceClient
{
    /// <summary>Represents an error for an unsuccessful HTTP request to a service.</summary>
    public sealed class JsonRpcRequestException : HttpRequestException
    {
        internal JsonRpcRequestException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }

        /// <summary>Gets the status code of the HTTP response.</summary>
        public HttpStatusCode StatusCode
        {
            get;
        }
    }
}