using System;
using System.Collections.Generic;
using System.Data.JsonRpc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Community.JsonRpc.ServiceClient.UnitTests.TestStubs
{
    internal sealed class TestJsonRpcClient : JsonRpcClient
    {
        public TestJsonRpcClient(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler = null)
            : base(new Uri("https://localhost", UriKind.Absolute), new HttpClient(new TestHttpHandler(handler)))
        {
        }

        protected override void VisitHttpRequestHeaders(HttpRequestHeaders headers)
        {
            VisitHttpRequestHeadersAction?.Invoke(headers);
        }

        protected override void VisitHttpResponseHeaders(HttpResponseHeaders headers)
        {
            VisitHttpResponseHeadersAction?.Invoke(headers);
        }

        public Task<IReadOnlyList<JsonRpcResponse>> PublicSendJsonRpcRequestsAsync(IReadOnlyList<JsonRpcRequest> requests, CancellationToken cancellationToken)
        {
            return SendJsonRpcRequestsAsync(requests, cancellationToken);
        }

        protected override Version HttpProtocolVersion
        {
            get => PublicHttpProtocolVersion;
        }

        public Version PublicHttpProtocolVersion
        {
            get;
            set;
        }

        public JsonRpcContractResolver PublicContractResolver
        {
            get => ContractResolver;
        }

        public Uri PublicServiceUri
        {
            get => ServiceUri;
        }

        public Action<HttpRequestHeaders> VisitHttpRequestHeadersAction
        {
            get;
            set;
        }

        public Action<HttpResponseHeaders> VisitHttpResponseHeadersAction
        {
            get;
            set;
        }
    }
}