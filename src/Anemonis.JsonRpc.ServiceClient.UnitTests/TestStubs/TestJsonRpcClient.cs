using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Anemonis.JsonRpc.ServiceClient.UnitTests.TestStubs
{
    internal sealed class TestJsonRpcClient : JsonRpcClient
    {
        public TestJsonRpcClient(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler = null)
            : base(new Uri("https://localhost"), CreateHttpInvoker(new TestHttpHandler(handler)))
        {
        }

        private static HttpMessageInvoker CreateHttpInvoker(HttpMessageHandler httpHandler)
        {
            var httpClient = new HttpClient(httpHandler);

            httpClient.DefaultRequestHeaders.ExpectContinue = false;
            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));

            return httpClient;
        }

        protected override void VisitHttpRequestMessage(HttpRequestMessage message)
        {
            VisitHttpRequestMessageAction?.Invoke(message);
        }

        protected override void VisitHttpResponseMessage(HttpResponseMessage message)
        {
            VisitHttpResponseMessageAction?.Invoke(message);
        }

        public JsonRpcId PublicGetUniqueRequestId()
        {
            return GetUniqueRequestId();
        }

        public Task<IReadOnlyList<JsonRpcResponse>> PublicSendJsonRpcRequestsAsync(IReadOnlyList<JsonRpcRequest> requests, CancellationToken cancellationToken)
        {
            return SendJsonRpcRequestsAsync(requests, cancellationToken);
        }

        public JsonRpcContractResolver PublicContractResolver
        {
            get => ContractResolver;
        }

        public Uri PublicServiceUri
        {
            get => ServiceUri;
        }

        public Action<HttpRequestMessage> VisitHttpRequestMessageAction
        {
            get;
            set;
        }

        public Action<HttpResponseMessage> VisitHttpResponseMessageAction
        {
            get;
            set;
        }
    }
}
