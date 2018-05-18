using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Community.JsonRpc.ServiceClient.Benchmarks.Internal
{
    internal sealed class JsonRpcClientBenchmarkHandler : HttpMessageHandler
    {
        private static readonly MediaTypeHeaderValue _mediaTypeHeaderValue = new MediaTypeHeaderValue("application/json");

        private readonly IReadOnlyDictionary<string, string> _contents;

        public JsonRpcClientBenchmarkHandler(IReadOnlyDictionary<string, string> contents)
        {
            if (contents == null)
            {
                throw new ArgumentNullException(nameof(contents));
            }

            _contents = contents;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var requestToken = JObject.Parse(await request.Content.ReadAsStringAsync());
            var responseContent = _contents[(string)requestToken["method"]];

            if (responseContent == null)
            {
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NoContent
                };
            }
            else
            {
                var content = new StringContent(responseContent.Replace("{id}", (string)requestToken["id"]));

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