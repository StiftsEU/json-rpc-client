using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Anemonis.JsonRpc.ServiceClient.Benchmarks.TestStubs
{
    internal class JsonRpcClientBenchmarkHandler : HttpMessageHandler
    {
        private static readonly MediaTypeHeaderValue _mediaTypeHeaderValue = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

        private readonly byte[] _content;

        public JsonRpcClientBenchmarkHandler(byte[] content = null)
        {
            _content = content;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var httpResponseMessage = default(HttpResponseMessage);

            if (_content == null)
            {
                httpResponseMessage = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NoContent
                };
            }
            else
            {
                var httpContent = new ByteArrayContent(_content);

                httpContent.Headers.ContentType = _mediaTypeHeaderValue;

                httpResponseMessage = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = httpContent
                };
            }

            return Task.FromResult(httpResponseMessage);
        }
    }
}
