using System;
using System.Net.Http;

namespace Community.JsonRpc.ServiceClient.UnitTests.Internal
{
    internal sealed class TestJsonRpcClientWithHttpVersion : JsonRpcClient
    {
        public TestJsonRpcClientWithHttpVersion(string serviceUri, HttpMessageInvoker httpInvoker, Version httpVersion)
            : base(serviceUri, httpInvoker)
        {
            HttpVersion = httpVersion;
        }

        protected override Version HttpVersion
        {
            get;
        }
    }
}