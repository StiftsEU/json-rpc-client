using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anemonis.JsonRpc.ServiceClient.UnitTests.TestStubs
{
    internal sealed class TestHttpHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _handler;

        public TestHttpHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler = null)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_handler == null)
            {
                throw new InvalidOperationException("Request processing is not available");
            }

            Assert.IsTrue(request.Headers.Contains("Accept"));
            Assert.IsTrue(request.Headers.Contains("Accept-Charset"));
            Assert.IsTrue(request.Headers.Contains("Date"));

            return _handler.Invoke(request);
        }
    }
}
