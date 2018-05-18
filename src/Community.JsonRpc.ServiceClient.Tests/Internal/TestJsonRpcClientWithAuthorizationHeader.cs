using System.Net.Http;
using System.Net.Http.Headers;

namespace Community.JsonRpc.ServiceClient.Tests.Internal
{
    internal sealed class TestJsonRpcClientWithAuthorizationHeader : JsonRpcClient
    {
        private readonly AuthenticationHeaderValue _authorizationHeader;

        public TestJsonRpcClientWithAuthorizationHeader(string serviceUri, HttpMessageInvoker httpInvoker, AuthenticationHeaderValue authorizationHeader)
            : base(serviceUri, httpInvoker)
        {
            _authorizationHeader = authorizationHeader;
        }

        protected override void VisitRequestHeaders(HttpRequestHeaders headers)
        {
            headers.Authorization = _authorizationHeader;
        }
    }
}