using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Community.JsonRpc.ServiceClient.Benchmarks.Internal
{
    /// <summary>A benchmark HTTP message handler for the <see cref="JsonRpcClient" />.</summary>
    internal sealed class JsonRpcClientBenchmarkHandler : HttpMessageHandler
    {
        private static readonly MediaTypeHeaderValue _mediaTypeHeaderValue = new MediaTypeHeaderValue("application/json");

        private readonly string _content;

        /// <summary>Initializes a new instance of the <see cref="JsonRpcClientBenchmarkHandler" /> class.</summary>
        /// <param name="content">The handler response.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content" /> is <see langword="null" />.</exception>
        public JsonRpcClientBenchmarkHandler(string content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            _content = content;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_content))
            {
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NoContent
                };
            }
            else
            {
                var requestToken = JObject.Parse(await request.Content.ReadAsStringAsync().ConfigureAwait(false));
                var content = new StringContent(_content.Replace("{id}", (string)requestToken["id"]));

                content.Headers.ContentType = _mediaTypeHeaderValue;

                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                };
            }
        }
    }
}