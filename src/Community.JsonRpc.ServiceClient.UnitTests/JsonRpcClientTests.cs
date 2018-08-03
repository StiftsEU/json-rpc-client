using System;
using System.Collections.Generic;
using System.Data.JsonRpc;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Community.JsonRpc.ServiceClient.UnitTests.TestStubs;
using Community.JsonRpc.ServiceClient.UnitTests.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Community.JsonRpc.ServiceClient.UnitTests
{
    [TestClass]
    public sealed class JsonRpcClientTests
    {
        [TestMethod]
        public void ConstructorWhenServiceUriIsStringAndIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcClient((string)null));
        }

        [TestMethod]
        public void ConstructorWhenServiceUriIsStringAndIsRelative()
        {
            Assert.ThrowsException<UriFormatException>(() =>
                new JsonRpcClient("/api"));
        }

        [TestMethod]
        public void ConstructorWhenServiceUriIsUriAndIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcClient((Uri)null));
        }

        [TestMethod]
        public void ConstructorWhenServiceUriIsUriAndIsRelative()
        {
            Assert.ThrowsException<ArgumentException>(() =>
                new JsonRpcClient(new Uri("/api", UriKind.Relative)));
        }

        [TestMethod]
        public void ConstructorWithInvokerWhenServiceUriIsStringAndIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcClient((string)null, new HttpClient(new TestHttpHandler())));
        }

        [TestMethod]
        public void ConstructorWithInvokerWhenServiceUriIsStringAndIsRelative()
        {
            Assert.ThrowsException<UriFormatException>(() =>
                new JsonRpcClient("/api", new HttpClient(new TestHttpHandler())));
        }

        [TestMethod]
        public void ConstructorWithInvokerWhenServiceUriIsStringAndInvokerIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcClient("https://localhost", null));
        }

        [TestMethod]
        public void ConstructorWithInvokerWhenServiceUriIsUriAndIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcClient((Uri)null, new HttpClient(new TestHttpHandler())));
        }

        [TestMethod]
        public void ConstructorWithInvokerWhenServiceUriIsUriAndIsRelative()
        {
            Assert.ThrowsException<ArgumentException>(() =>
                new JsonRpcClient(new Uri("/api", UriKind.Relative), new HttpClient(new TestHttpHandler())));
        }

        [TestMethod]
        public void ConstructorWithInvokerWhenServiceUriIsUriAndInvokerIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcClient(new Uri("https://localhost", UriKind.Absolute), null));
        }

        //###################################################################################################

        [TestMethod]
        public async Task InvokeWhenMethodIsNull()
        {
            var client = new TestJsonRpcClient();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync((string)null));
        }

        [TestMethod]
        public async Task InvokeWhenMethodIsNullAndParametersAreByPosition()
        {
            var client = new TestJsonRpcClient();
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync((string)null, parameters));
        }

        [TestMethod]
        public async Task InvokeWhenMethodIsNullAndParametersAreByName()
        {
            var client = new TestJsonRpcClient();
            var parameters = new Dictionary<string, object> { ["a"] = 1L, ["b"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync((string)null, parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultWhenMethodIsNull()
        {
            var client = new TestJsonRpcClient();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>((string)null));
        }

        [TestMethod]
        public async Task InvokeWithResultWhenMethodIsNullAndParametersAreByPosition()
        {
            var client = new TestJsonRpcClient();
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>((string)null, parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultWhenMethodIsNullAndParametersAreByName()
        {
            var client = new TestJsonRpcClient();
            var parameters = new Dictionary<string, object> { ["a"] = 1L, ["b"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>((string)null, parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndIdentifierWhenMethodIsNull()
        {
            var client = new TestJsonRpcClient();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>((string)null, default(JsonRpcId)));
        }

        [TestMethod]
        public async Task InvokeWithResultAndIdentifierWhenMethodIsNullAndParametersAreByPosition()
        {
            var client = new TestJsonRpcClient();
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>((string)null, default(JsonRpcId), parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndIdentifierWhenMethodIsNullAndParametersAreByName()
        {
            var client = new TestJsonRpcClient();
            var parameters = new Dictionary<string, object> { ["a"] = 1L, ["b"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>((string)null, default(JsonRpcId), parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataWhenMethodIsNull()
        {
            var client = new TestJsonRpcClient();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long, long>((string)null));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataWhenMethodIsNullAndParametersAreByPosition()
        {
            var client = new TestJsonRpcClient();
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long, long>((string)null, parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataWhenMethodIsNullAndParametersAreByName()
        {
            var client = new TestJsonRpcClient();
            var parameters = new Dictionary<string, object> { ["a"] = 1L, ["b"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long, long>((string)null, parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataAndIdentifierWhenMethodIsNull()
        {
            var client = new TestJsonRpcClient();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long, long>((string)null, default(JsonRpcId)));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataAndIdentifierWhenMethodIsNullAndParametersAreByPosition()
        {
            var client = new TestJsonRpcClient();
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long, long>((string)null, default(JsonRpcId), parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataAndIdentifierWhenMethodIsNullAndParametersAreByName()
        {
            var client = new TestJsonRpcClient();
            var parameters = new Dictionary<string, object> { ["a"] = 1L, ["b"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long, long>((string)null, default(JsonRpcId), parameters));
        }

        //###################################################################################################

        [TestMethod]
        public async Task InvokeWhenMethodIsSystem()
        {
            var client = new TestJsonRpcClient();

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync("rpc.m"));
        }

        [TestMethod]
        public async Task InvokeWhenMethodIsSystemAndParametersAreByPosition()
        {
            var client = new TestJsonRpcClient();
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync("rpc.m", parameters));
        }

        [TestMethod]
        public async Task InvokeWhenMethodIsSystemAndParametersAreByName()
        {
            var client = new TestJsonRpcClient();
            var parameters = new Dictionary<string, object> { ["a"] = 1L, ["b"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync("rpc.m", parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultWhenMethodIsSystem()
        {
            var client = new TestJsonRpcClient();

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("rpc.m"));
        }

        [TestMethod]
        public async Task InvokeWithResultWhenMethodIsSystemAndParametersAreByPosition()
        {
            var client = new TestJsonRpcClient();
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("rpc.m", parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultWhenMethodIsSystemAndParametersAreByName()
        {
            var client = new TestJsonRpcClient();
            var parameters = new Dictionary<string, object> { ["a"] = 1L, ["b"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("rpc.m", parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndIdentifierWhenMethodIsSystem()
        {
            var client = new TestJsonRpcClient();

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("rpc.m", default(JsonRpcId)));
        }

        [TestMethod]
        public async Task InvokeWithResultAndIdentifierWhenMethodIsSystemAndParametersAreByPosition()
        {
            var client = new TestJsonRpcClient();
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("rpc.m", default(JsonRpcId), parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndIdentifierWhenMethodIsSystemAndParametersAreByName()
        {
            var client = new TestJsonRpcClient();
            var parameters = new Dictionary<string, object> { ["a"] = 1L, ["b"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("rpc.m", default(JsonRpcId), parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataWhenMethodIsSystem()
        {
            var client = new TestJsonRpcClient();

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long, long>("rpc.m"));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataWhenMethodIsSystemAndParametersAreByPosition()
        {
            var client = new TestJsonRpcClient();
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long, long>("rpc.m", parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataWhenMethodIsSystemAndParametersAreByName()
        {
            var client = new TestJsonRpcClient();
            var parameters = new Dictionary<string, object> { ["a"] = 1L, ["b"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long, long>("rpc.m", default(JsonRpcId), parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataAndIdentifierWhenMethodIsSystem()
        {
            var client = new TestJsonRpcClient();

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long, long>("rpc.m", default(JsonRpcId)));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataAndIdentifierWhenMethodIsSystemAndParametersAreByPosition()
        {
            var client = new TestJsonRpcClient();
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long, long>("rpc.m", default(JsonRpcId), parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataAndIdentifierWhenMethodIsSystemAndParametersAreByName()
        {
            var client = new TestJsonRpcClient();
            var parameters = new Dictionary<string, object> { ["a"] = 1L, ["b"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long, long>("rpc.m", parameters));
        }

        //###################################################################################################

        [TestMethod]
        public async Task InvokeWithParametersByPositionWhenParametersIsNull()
        {
            var client = new TestJsonRpcClient();
            var parameters = (object[])null;

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync("m", parameters));
        }

        [TestMethod]
        public async Task InvokeWithParametersByNameWhenParametersIsNull()
        {
            var client = new TestJsonRpcClient();
            var parameters = (Dictionary<string, object>)null;

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync("m", parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndParametersByPositionWhenParametersIsNull()
        {
            var client = new TestJsonRpcClient();
            var parameters = (object[])null;

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>("m", parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndParametersByNameWhenParametersIsNull()
        {
            var client = new TestJsonRpcClient();
            var parameters = (Dictionary<string, object>)null;

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>("m", parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndIdentifierAndParametersByPositionWhenParametersIsNull()
        {
            var client = new TestJsonRpcClient();
            var parameters = (object[])null;

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>("m", 0L, parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndIdentifierAndParametersByNameWhenParametersIsNull()
        {
            var client = new TestJsonRpcClient();
            var parameters = (Dictionary<string, object>)null;

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>("m", 0L, parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataAndIdentifierAndParametersByPositionWhenParametersIsNull()
        {
            var client = new TestJsonRpcClient();
            var parameters = (object[])null;

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long, long>("m", 0L, parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataAndIdentifierAndParametersByNameWhenParametersIsNull()
        {
            var client = new TestJsonRpcClient();
            var parameters = (Dictionary<string, object>)null;

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long, long>("m", 0L, parameters));
        }

        //###################################################################################################

        [TestMethod]
        public async Task InvokeWithResultWhenIdentifierIsEmpty()
        {
            var client = new TestJsonRpcClient();

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("m", default(JsonRpcId)));
        }

        [TestMethod]
        public async Task InvokeWithResultWhenIdentifierIsEmptyAndParametersAreByPosition()
        {
            var client = new TestJsonRpcClient();
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("m", default(JsonRpcId), parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultWhenIdentifierIsEmptyAndParametersAreByName()
        {
            var client = new TestJsonRpcClient();
            var parameters = new Dictionary<string, object> { ["a"] = 1L, ["b"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("m", default(JsonRpcId), parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataWhenIdentifierIsEmpty()
        {
            var client = new TestJsonRpcClient();

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long, long>("m", default(JsonRpcId)));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataWhenIdentifierIsEmptyAndParametersAreByPosition()
        {
            var client = new TestJsonRpcClient();
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long, long>("m", default(JsonRpcId), parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataWhenIdentifierIsEmptyAndParametersAreByName()
        {
            var client = new TestJsonRpcClient();
            var parameters = new Dictionary<string, object> { ["a"] = 1L, ["b"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long, long>("m", default(JsonRpcId), parameters));
        }

        //###################################################################################################

        [TestMethod]
        public async Task InvokeWhenCancellationTokenIsCancelled()
        {
            var client = new TestJsonRpcClient();
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync("m", cancellationTokenSource.Token));
        }

        [TestMethod]
        public async Task InvokeWhenCancellationTokenIsCancelledAndParametersAreByPosition()
        {
            var client = new TestJsonRpcClient();
            var parameters = new object[] { 1L, 2L };
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync("m", parameters, cancellationTokenSource.Token));
        }

        [TestMethod]
        public async Task InvokeWhenCancellationTokenIsCancelledAndParametersAreByName()
        {
            var client = new TestJsonRpcClient();
            var parameters = new Dictionary<string, object> { ["a"] = 1L, ["b"] = 2L };
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync("m", parameters, cancellationTokenSource.Token));
        }

        [TestMethod]
        public async Task InvokeWithResultWhenCancellationTokenIsCancelled()
        {
            var client = new TestJsonRpcClient();
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long>("m", cancellationTokenSource.Token));
        }

        [TestMethod]
        public async Task InvokeWithResultWhenCancellationTokenIsCancelledAndParametersAreByPosition()
        {
            var client = new TestJsonRpcClient();
            var parameters = new object[] { 1L, 2L };
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long>("m", parameters, cancellationTokenSource.Token));
        }

        [TestMethod]
        public async Task InvokeWithResultWhenCancellationTokenIsCancelledAndParametersAreByName()
        {
            var client = new TestJsonRpcClient();
            var parameters = new Dictionary<string, object> { ["a"] = 1L, ["b"] = 2L };
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long>("m", parameters, cancellationTokenSource.Token));
        }

        [TestMethod]
        public async Task InvokeWithResultAndIdentifierWhenCancellationTokenIsCancelled()
        {
            var client = new TestJsonRpcClient();
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long>("m", 0L, cancellationTokenSource.Token));
        }

        [TestMethod]
        public async Task InvokeWithResultAndIdentifierWhenCancellationTokenIsCancelledAndParametersAreByPosition()
        {
            var client = new TestJsonRpcClient();
            var parameters = new object[] { 1L, 2L };
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long>("m", 0L, parameters, cancellationTokenSource.Token));
        }

        [TestMethod]
        public async Task InvokeWithResultAndIdentifierWhenCancellationTokenIsCancelledAndParametersAreByName()
        {
            var client = new TestJsonRpcClient();
            var parameters = new Dictionary<string, object> { ["a"] = 1L, ["b"] = 2L };
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long>("m", 0L, parameters, cancellationTokenSource.Token));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataWhenCancellationTokenIsCancelled()
        {
            var client = new TestJsonRpcClient();
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long, long>("m", cancellationTokenSource.Token));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataWhenCancellationTokenIsCancelledAndParametersAreByPosition()
        {
            var client = new TestJsonRpcClient();
            var parameters = new object[] { 1L, 2L };
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long, long>("m", parameters, cancellationTokenSource.Token));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataWhenCancellationTokenIsCancelledAndParametersAreByName()
        {
            var client = new TestJsonRpcClient();
            var parameters = new Dictionary<string, object> { ["a"] = 1L, ["b"] = 2L };
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long, long>("m", parameters, cancellationTokenSource.Token));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataAndIdentifierWhenCancellationTokenIsCancelled()
        {
            var client = new TestJsonRpcClient();
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long, long>("m", 0L, cancellationTokenSource.Token));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataAndIdentifierWhenCancellationTokenIsCancelledAndParametersAreByPosition()
        {
            var client = new TestJsonRpcClient();
            var parameters = new object[] { 1L, 2L };
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long, long>("m", 0L, parameters, cancellationTokenSource.Token));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataAndIdentifierWhenCancellationTokenIsCancelledAndParametersAreByName()
        {
            var client = new TestJsonRpcClient();
            var parameters = new Dictionary<string, object> { ["a"] = 1L, ["b"] = 2L };
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long, long>("m", 0L, parameters, cancellationTokenSource.Token));
        }

        //###################################################################################################

        [TestMethod]
        public async Task InvokeWhenHttpStatusCodeIsInvalid()
        {
            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                };

                return Task.FromResult(message);
            });

            using (var client = new TestJsonRpcClient(handler))
            {
                var exception = await Assert.ThrowsExceptionAsync<JsonRpcRequestException>(() =>
                    client.InvokeAsync("m"));

                Assert.AreEqual(HttpStatusCode.BadRequest, exception.StatusCode);
            }
        }

        [TestMethod]
        public async Task InvokeWhenHttpContentTypeIsEmpty()
        {
            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var content = new ByteArrayContent(Array.Empty<byte>());

                content.Headers.ContentType = null;
                content.Headers.ContentLength = 0L;

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                };

                return Task.FromResult(message);
            });

            using (var client = new TestJsonRpcClient(handler))
            {
                var exception = await Assert.ThrowsExceptionAsync<JsonRpcRequestException>(() =>
                    client.InvokeAsync<long>("m"));

                Assert.AreEqual(HttpStatusCode.OK, exception.StatusCode);
            }
        }

        [TestMethod]
        public async Task InvokeWhenHttpContentTypeIsInvalid()
        {
            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var content = new ByteArrayContent(Array.Empty<byte>());

                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                content.Headers.ContentLength = 0L;

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                };

                return Task.FromResult(message);
            });

            using (var client = new TestJsonRpcClient(handler))
            {
                var exception = await Assert.ThrowsExceptionAsync<JsonRpcRequestException>(() =>
                    client.InvokeAsync<long>("m"));

                Assert.AreEqual(HttpStatusCode.OK, exception.StatusCode);
            }
        }

        [TestMethod]
        public async Task InvokeWhenResponseIsBatch()
        {
            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var responseObject0 = new JObject();

                responseObject0["jsonrpc"] = "2.0";
                responseObject0["id"] = 0L;
                responseObject0["result"] = null;

                var responseObject1 = new JObject();

                responseObject1["jsonrpc"] = "2.0";
                responseObject1["id"] = 1L;
                responseObject1["result"] = null;

                var responseBatch = new JArray(responseObject0, responseObject1);
                var contentBytes = Encoding.UTF8.GetBytes(responseBatch.ToString());
                var content = new ByteArrayContent(contentBytes);

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Headers.ContentLength = contentBytes.Length;

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                };

                return Task.FromResult(message);
            });

            using (var client = new TestJsonRpcClient(handler))
            {
                await Assert.ThrowsExceptionAsync<JsonRpcContractException>(() =>
                    client.InvokeAsync<long>("m"));
            }
        }

        [TestMethod]
        public async Task InvokeWhenResponseIdIsInvalid()
        {
            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var contentBytes = Encoding.UTF8.GetBytes(EmbeddedResourceManager.GetString("Assets.result_true_id_invalid.json"));
                var content = new ByteArrayContent(contentBytes);

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Headers.ContentLength = contentBytes.Length;

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                };

                return Task.FromResult(message);
            });

            using (var client = new TestJsonRpcClient(handler))
            {
                var exception = await Assert.ThrowsExceptionAsync<JsonRpcContractException>(() =>
                    client.InvokeAsync<long>("m"));

                Assert.AreNotEqual(string.Empty, exception.RequestId);
            }
        }

        [TestMethod]
        public async Task InvokeWhenResultTypeIsInvalid()
        {
            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)(async (request) =>
            {
                var requestString = await request.Content.ReadAsStringAsync();
                var requestObject = JObject.Parse(requestString);
                var responseObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.result_type_invalid.json"));

                responseObject["id"] = requestObject["id"];

                var contentBytes = Encoding.UTF8.GetBytes(responseObject.ToString());
                var content = new ByteArrayContent(contentBytes);

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Headers.ContentLength = contentBytes.Length;

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                };

                return message;
            });

            using (var client = new TestJsonRpcClient(handler))
            {
                var exception = await Assert.ThrowsExceptionAsync<JsonRpcContractException>(() =>
                    client.InvokeAsync<long>("m"));

                Assert.AreNotEqual(string.Empty, exception.RequestId);
            }
        }

        [TestMethod]
        public async Task InvokeWhenResponseContractIsInvalidAndResponseIsNotExpected()
        {
            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)(async (request) =>
            {
                var requestString = await request.Content.ReadAsStringAsync();
                var requestObject = JObject.Parse(requestString);
                var responseObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.result_true_unexpected.json"));

                responseObject["id"] = requestObject["id"];

                var contentBytes = Encoding.UTF8.GetBytes(responseObject.ToString());
                var content = new ByteArrayContent(contentBytes);

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Headers.ContentLength = contentBytes.Length;

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                };

                return message;
            });

            using (var client = new TestJsonRpcClient(handler))
            {
                var exception = await Assert.ThrowsExceptionAsync<JsonRpcContractException>(() =>
                    client.InvokeAsync("m"));

                Assert.AreEqual(string.Empty, exception.RequestId);
            }
        }

        [TestMethod]
        public async Task InvokeWhenResponseContractIsInvalidAndResponseIsExpected()
        {
            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NoContent
                };

                return Task.FromResult(message);
            });

            using (var client = new TestJsonRpcClient(handler))
            {
                var exception = await Assert.ThrowsExceptionAsync<JsonRpcContractException>(() =>
                    client.InvokeAsync<long>("m"));

                Assert.AreNotEqual(string.Empty, exception.RequestId);
            }
        }

        [TestMethod]
        public async Task InvokeWhenResponseIsNotExpected()
        {
            var requestAcceptHeader = default(HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue>);

            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                requestAcceptHeader = request.Headers.Accept;

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NoContent
                };

                return Task.FromResult(message);
            });

            using (var client = new TestJsonRpcClient(handler))
            {
                await client.InvokeAsync("m");

                Assert.IsNotNull(requestAcceptHeader);

                CollectionAssert.Contains(requestAcceptHeader.ToArray(), new MediaTypeWithQualityHeaderValue("application/json"));
            }
        }

        [TestMethod]
        public async Task InvokeWhenResponseIsExpected()
        {
            var requestAcceptHeader = default(HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue>);

            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)(async (request) =>
            {
                requestAcceptHeader = request.Headers.Accept;

                var requestString = await request.Content.ReadAsStringAsync();
                var requestObject = JObject.Parse(requestString);
                var responseObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.result_true_success_true.json"));

                responseObject["id"] = requestObject["id"];

                var contentBytes = Encoding.UTF8.GetBytes(responseObject.ToString());
                var content = new ByteArrayContent(contentBytes);

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Headers.ContentLength = contentBytes.Length;

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                };

                return message;
            });

            using (var client = new TestJsonRpcClient(handler))
            {
                var result = await client.InvokeAsync<long>("m");

                Assert.IsNotNull(requestAcceptHeader);

                CollectionAssert.Contains(requestAcceptHeader.ToArray(), new MediaTypeWithQualityHeaderValue("application/json"));

                Assert.AreEqual(1L, result);
            }
        }

        [TestMethod]
        public async Task InvokeWhenResponseWithErrorIsExpected()
        {
            var requestAcceptHeader = default(HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue>);

            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)(async (request) =>
            {
                requestAcceptHeader = request.Headers.Accept;

                var requestString = await request.Content.ReadAsStringAsync();
                var requestObject = JObject.Parse(requestString);
                var responseObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.result_true_success_false.json"));

                responseObject["id"] = requestObject["id"];

                var contentBytes = Encoding.UTF8.GetBytes(responseObject.ToString());
                var content = new ByteArrayContent(contentBytes);

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Headers.ContentLength = contentBytes.Length;

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                };

                return message;
            });

            using (var client = new TestJsonRpcClient(handler))
            {
                var exception = await Assert.ThrowsExceptionAsync<JsonRpcServiceException>(() =>
                    client.InvokeAsync<long, long>("m"));

                Assert.IsNotNull(requestAcceptHeader);

                CollectionAssert.Contains(requestAcceptHeader.ToArray(), new MediaTypeWithQualityHeaderValue("application/json"));

                Assert.AreEqual(1L, exception.Code);
                Assert.AreEqual("e", exception.Message);
                Assert.IsTrue(exception.HasErrorData);
                Assert.AreEqual(exception.ErrorData, 1L);
            }
        }

        //###################################################################################################

        [TestMethod]
        public async Task InvokeWhenHttpProtocolVersion()
        {
            var httpProtocolVersion = new Version(2, 0);

            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                Assert.AreEqual(httpProtocolVersion, request.Version);

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NoContent
                };

                return Task.FromResult(message);
            });

            using (var client = new TestJsonRpcClient(handler))
            {
                client.PublicHttpProtocolVersion = httpProtocolVersion;

                await client.InvokeAsync("m");
            }
        }

        [TestMethod]
        public async Task InvokeWithCustomRequestHeader()
        {
            var authorizationHeader = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes("PASSWORD")));

            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                Assert.IsNotNull(request.Headers.Authorization);
                Assert.AreEqual(authorizationHeader.Scheme, request.Headers.Authorization.Scheme);
                Assert.AreEqual(authorizationHeader.Parameter, request.Headers.Authorization.Parameter);

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NoContent
                };

                return Task.FromResult(message);
            });

            using (var client = new TestJsonRpcClient(handler))
            {
                client.VisitHttpRequestHeadersAction = headers => headers.Authorization = authorizationHeader;

                await client.InvokeAsync("m");
            }
        }

        //###################################################################################################

        [TestMethod]
        public void GetServiceUri()
        {
            using (var client = new TestJsonRpcClient())
            {
                Assert.IsNotNull(client.PublicServiceUri);
                Assert.AreEqual("https://localhost", client.PublicServiceUri.OriginalString);
            }
        }

        [TestMethod]
        public void GetContractResolver()
        {
            using (var client = new TestJsonRpcClient())
            {
                Assert.IsNotNull(client.PublicContractResolver);
            }
        }

        //###################################################################################################

        [TestMethod]
        public async Task SendJsonRpcRequestsAsyncWhenRequestsIsNull()
        {
            using (var client = new TestJsonRpcClient())
            {
                await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                     client.PublicSendJsonRpcRequestsAsync(null, default));
            }
        }

        [TestMethod]
        public async Task SendJsonRpcRequestsAsyncWhenRequestsHasDuplicateIdentifiers()
        {
            var requests = new[]
            {
                new JsonRpcRequest("m", 0L),
                new JsonRpcRequest("m", 0L)
            };

            using (var client = new TestJsonRpcClient())
            {
                await Assert.ThrowsExceptionAsync<JsonRpcContractException>(() =>
                     client.PublicSendJsonRpcRequestsAsync(requests, default));
            }
        }

        [TestMethod]
        public async Task SendJsonRpcRequestsAsyncWhenHttpStatusCodeIsInvalid()
        {
            var requests = new[]
            {
                new JsonRpcRequest("m", 0L),
                new JsonRpcRequest("m", 1L)
            };

            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var contentBytes = Encoding.UTF8.GetBytes(string.Empty);
                var content = new ByteArrayContent(contentBytes);

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Headers.ContentLength = contentBytes.Length;

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                };

                return Task.FromResult(message);
            });

            using (var client = new TestJsonRpcClient(handler))
            {
                client.PublicContractResolver.AddResponseContract("m", new JsonRpcResponseContract(typeof(string)));
                client.PublicContractResolver.AddResponseBinding(0L, "m");
                client.PublicContractResolver.AddResponseBinding(1L, "m");

                var exception = await Assert.ThrowsExceptionAsync<JsonRpcRequestException>(() =>
                     client.PublicSendJsonRpcRequestsAsync(requests, default));

                Assert.AreEqual(HttpStatusCode.BadRequest, exception.StatusCode);
            }
        }

        [TestMethod]
        public async Task SendJsonRpcRequestsAsyncWhenHttpContentTypeIsEmpty()
        {
            var requests = new[]
            {
                new JsonRpcRequest("m", 0L),
                new JsonRpcRequest("m", 1L)
            };

            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var content = new ByteArrayContent(Array.Empty<byte>());

                content.Headers.ContentType = null;
                content.Headers.ContentLength = 0L;

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                };

                return Task.FromResult(message);
            });

            using (var client = new TestJsonRpcClient(handler))
            {
                var exception = await Assert.ThrowsExceptionAsync<JsonRpcRequestException>(() =>
                     client.PublicSendJsonRpcRequestsAsync(requests, default));

                Assert.AreEqual(HttpStatusCode.OK, exception.StatusCode);
            }
        }

        [TestMethod]
        public async Task SendJsonRpcRequestsAsyncWhenHttpContentTypeIsInvalid()
        {
            var requests = new[]
            {
                new JsonRpcRequest("m", 0L),
                new JsonRpcRequest("m", 1L)
            };

            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var content = new ByteArrayContent(Array.Empty<byte>());

                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                content.Headers.ContentLength = 0L;

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                };

                return Task.FromResult(message);
            });

            using (var client = new TestJsonRpcClient(handler))
            {
                var exception = await Assert.ThrowsExceptionAsync<JsonRpcRequestException>(() =>
                     client.PublicSendJsonRpcRequestsAsync(requests, default));

                Assert.AreEqual(HttpStatusCode.OK, exception.StatusCode);
            }
        }

        [TestMethod]
        public async Task SendJsonRpcRequestsAsyncWhenResponsesDoNotMatch()
        {
            var requests = new[]
            {
                new JsonRpcRequest("m", 0L),
                new JsonRpcRequest("m", 1L)
            };

            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var responseObject = new JObject();

                responseObject["jsonrpc"] = "2.0";
                responseObject["id"] = 0L;
                responseObject["result"] = null;

                var responseBatch = new JArray(responseObject);
                var contentBytes = Encoding.UTF8.GetBytes(responseBatch.ToString());
                var content = new ByteArrayContent(contentBytes);

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Headers.ContentLength = contentBytes.Length;

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                };

                return Task.FromResult(message);
            });

            using (var client = new TestJsonRpcClient(handler))
            {
                client.PublicContractResolver.AddResponseContract("m", new JsonRpcResponseContract(typeof(string)));
                client.PublicContractResolver.AddResponseBinding(0L, "m");
                client.PublicContractResolver.AddResponseBinding(1L, "m");

                await Assert.ThrowsExceptionAsync<JsonRpcContractException>(() =>
                     client.PublicSendJsonRpcRequestsAsync(requests, default));
            }
        }

        [TestMethod]
        public async Task SendJsonRpcRequestsAsyncWhenResponseIsNotBatchAndInvalid()
        {
            var requests = new[]
            {
                new JsonRpcRequest("m", 0L)
            };

            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var responseObject = new JObject();

                responseObject["jsonrpc"] = "0.0";
                responseObject["id"] = 0L;
                responseObject["result"] = null;

                var contentBytes = Encoding.UTF8.GetBytes(responseObject.ToString());
                var content = new ByteArrayContent(contentBytes);

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Headers.ContentLength = contentBytes.Length;

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                };

                return Task.FromResult(message);
            });

            using (var client = new TestJsonRpcClient(handler))
            {
                client.PublicContractResolver.AddResponseContract("m", new JsonRpcResponseContract(typeof(string)));
                client.PublicContractResolver.AddResponseBinding(0L, "m");

                await Assert.ThrowsExceptionAsync<JsonRpcContractException>(() =>
                     client.PublicSendJsonRpcRequestsAsync(requests, default));
            }
        }

        [TestMethod]
        public async Task SendJsonRpcRequestsAsyncWhenResponseIsNotBatchAndHasValidResult()
        {
            var requests = new[]
            {
                new JsonRpcRequest("m", 0L)
            };

            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var responseObject = new JObject();

                responseObject["jsonrpc"] = "2.0";
                responseObject["id"] = 0L;
                responseObject["result"] = null;

                var contentBytes = Encoding.UTF8.GetBytes(responseObject.ToString());
                var content = new ByteArrayContent(contentBytes);

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Headers.ContentLength = contentBytes.Length;

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                };

                return Task.FromResult(message);
            });

            using (var client = new TestJsonRpcClient(handler))
            {
                client.PublicContractResolver.AddResponseContract("m", new JsonRpcResponseContract(typeof(string)));
                client.PublicContractResolver.AddResponseBinding(0L, "m");

                await Assert.ThrowsExceptionAsync<JsonRpcContractException>(() =>
                     client.PublicSendJsonRpcRequestsAsync(requests, default));
            }
        }

        [TestMethod]
        public async Task SendJsonRpcRequestsAsyncWhenResponseIsInvalid()
        {
            var requests = new[]
            {
                new JsonRpcRequest("m", 0L),
                new JsonRpcRequest("m", 1L)
            };

            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var responseObject0 = new JObject();

                responseObject0["jsonrpc"] = "2.0";
                responseObject0["id"] = 0L;
                responseObject0["result"] = null;

                var responseObject1 = new JObject();

                responseObject1["jsonrpc"] = "2.0";
                responseObject1["id"] = 1L;

                var responseBatch = new JArray(responseObject0, responseObject1);
                var contentBytes = Encoding.UTF8.GetBytes(responseBatch.ToString());
                var content = new ByteArrayContent(contentBytes);

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Headers.ContentLength = contentBytes.Length;

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                };

                return Task.FromResult(message);
            });

            using (var client = new TestJsonRpcClient(handler))
            {
                client.PublicContractResolver.AddResponseContract("m", new JsonRpcResponseContract(typeof(string)));
                client.PublicContractResolver.AddResponseBinding(0L, "m");
                client.PublicContractResolver.AddResponseBinding(1L, "m");

                var exception = await Assert.ThrowsExceptionAsync<AggregateException>(() =>
                     client.PublicSendJsonRpcRequestsAsync(requests, default));

                Assert.IsNotNull(exception);
                Assert.IsNotNull(exception.InnerExceptions);
                Assert.AreEqual(1, exception.InnerExceptions.Count);
                Assert.IsInstanceOfType(exception.InnerExceptions[0], typeof(JsonRpcContractException));
            }
        }

        [TestMethod]
        public async Task SendJsonRpcRequestsAsyncWhenRequestsAreNotifications()
        {
            var requests = new[]
            {
                new JsonRpcRequest("m"),
                new JsonRpcRequest("m")
            };

            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var contentBytes = Encoding.UTF8.GetBytes(string.Empty);
                var content = new ByteArrayContent(contentBytes);

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Headers.ContentLength = contentBytes.Length;

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NoContent
                };

                return Task.FromResult(message);
            });

            using (var client = new TestJsonRpcClient(handler))
            {
                client.PublicContractResolver.AddResponseContract("m", new JsonRpcResponseContract(typeof(string)));
                client.PublicContractResolver.AddResponseBinding(0L, "m");
                client.PublicContractResolver.AddResponseBinding(1L, "m");

                var responses = await client.PublicSendJsonRpcRequestsAsync(requests, default);

                Assert.IsNotNull(responses);
                Assert.AreEqual(0, responses.Count);
            }
        }

        [TestMethod]
        public async Task SendJsonRpcRequestsAsync()
        {
            var requests = new[]
            {
                new JsonRpcRequest("m", 0L),
                new JsonRpcRequest("m", 1L)
            };

            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var responseObject0 = new JObject();

                responseObject0["jsonrpc"] = "2.0";
                responseObject0["id"] = 0L;
                responseObject0["result"] = null;

                var responseObject1 = new JObject();

                responseObject1["jsonrpc"] = "2.0";
                responseObject1["id"] = 1L;
                responseObject1["result"] = null;

                var responseBatch = new JArray(responseObject0, responseObject1);
                var contentBytes = Encoding.UTF8.GetBytes(responseBatch.ToString());
                var content = new ByteArrayContent(contentBytes);

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Headers.ContentLength = contentBytes.Length;

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                };

                return Task.FromResult(message);
            });

            using (var client = new TestJsonRpcClient(handler))
            {
                client.PublicContractResolver.AddResponseContract("m", new JsonRpcResponseContract(typeof(string)));
                client.PublicContractResolver.AddResponseBinding(0L, "m");
                client.PublicContractResolver.AddResponseBinding(1L, "m");

                var responses = await client.PublicSendJsonRpcRequestsAsync(requests, default);

                Assert.IsNotNull(responses);
                Assert.AreEqual(2, responses.Count);
            }
        }
    }
}