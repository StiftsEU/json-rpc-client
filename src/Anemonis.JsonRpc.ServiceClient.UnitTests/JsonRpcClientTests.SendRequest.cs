using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Anemonis.JsonRpc.ServiceClient.UnitTests.Resources;
using Anemonis.JsonRpc.ServiceClient.UnitTests.TestStubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Anemonis.JsonRpc.ServiceClient.UnitTests
{
    public partial class JsonRpcClientTests
    {
        [TestMethod]
        public async Task InvokeAsyncWhenMethodIsNull()
        {
            var client = new TestJsonRpcClient();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync((string)null));
        }

        [TestMethod]
        public async Task InvokeAsyncWhenMethodIsNullAndParametersAreByPosition()
        {
            var client = new TestJsonRpcClient();
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync((string)null, parameters));
        }

        [TestMethod]
        public async Task InvokeAsyncWhenMethodIsNullAndParametersAreByName()
        {
            var client = new TestJsonRpcClient();
            var parameters = new Dictionary<string, object> { ["a"] = 1L, ["b"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync((string)null, parameters));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultWhenMethodIsNull()
        {
            var client = new TestJsonRpcClient();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>((string)null));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultWhenMethodIsNullAndParametersAreByPosition()
        {
            var client = new TestJsonRpcClient();
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>((string)null, parameters));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultWhenMethodIsNullAndParametersAreByName()
        {
            var client = new TestJsonRpcClient();
            var parameters = new Dictionary<string, object> { ["a"] = 1L, ["b"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>((string)null, parameters));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndIdentifierWhenMethodIsNull()
        {
            var client = new TestJsonRpcClient();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>((string)null, default(JsonRpcId)));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndIdentifierWhenMethodIsNullAndParametersAreByPosition()
        {
            var client = new TestJsonRpcClient();
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>((string)null, default(JsonRpcId), parameters));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndIdentifierWhenMethodIsNullAndParametersAreByName()
        {
            var client = new TestJsonRpcClient();
            var parameters = new Dictionary<string, object> { ["a"] = 1L, ["b"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>((string)null, default(JsonRpcId), parameters));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndErrorDataWhenMethodIsNull()
        {
            var client = new TestJsonRpcClient();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long, long>((string)null));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndErrorDataWhenMethodIsNullAndParametersAreByPosition()
        {
            var client = new TestJsonRpcClient();
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long, long>((string)null, parameters));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndErrorDataWhenMethodIsNullAndParametersAreByName()
        {
            var client = new TestJsonRpcClient();
            var parameters = new Dictionary<string, object> { ["a"] = 1L, ["b"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long, long>((string)null, parameters));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndErrorDataAndIdentifierWhenMethodIsNull()
        {
            var client = new TestJsonRpcClient();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long, long>((string)null, default(JsonRpcId)));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndErrorDataAndIdentifierWhenMethodIsNullAndParametersAreByPosition()
        {
            var client = new TestJsonRpcClient();
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long, long>((string)null, default(JsonRpcId), parameters));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndErrorDataAndIdentifierWhenMethodIsNullAndParametersAreByName()
        {
            var client = new TestJsonRpcClient();
            var parameters = new Dictionary<string, object> { ["a"] = 1L, ["b"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long, long>((string)null, default(JsonRpcId), parameters));
        }

        //###################################################################################################

        [TestMethod]
        public async Task InvokeAsyncWhenMethodIsSystem()
        {
            var client = new TestJsonRpcClient();

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync("rpc.m"));
        }

        [TestMethod]
        public async Task InvokeAsyncWhenMethodIsSystemAndParametersAreByPosition()
        {
            var client = new TestJsonRpcClient();
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync("rpc.m", parameters));
        }

        [TestMethod]
        public async Task InvokeAsyncWhenMethodIsSystemAndParametersAreByName()
        {
            var client = new TestJsonRpcClient();
            var parameters = new Dictionary<string, object> { ["a"] = 1L, ["b"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync("rpc.m", parameters));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultWhenMethodIsSystem()
        {
            var client = new TestJsonRpcClient();

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("rpc.m"));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultWhenMethodIsSystemAndParametersAreByPosition()
        {
            var client = new TestJsonRpcClient();
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("rpc.m", parameters));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultWhenMethodIsSystemAndParametersAreByName()
        {
            var client = new TestJsonRpcClient();
            var parameters = new Dictionary<string, object> { ["a"] = 1L, ["b"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("rpc.m", parameters));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndIdentifierWhenMethodIsSystem()
        {
            var client = new TestJsonRpcClient();

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("rpc.m", default(JsonRpcId)));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndIdentifierWhenMethodIsSystemAndParametersAreByPosition()
        {
            var client = new TestJsonRpcClient();
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("rpc.m", default(JsonRpcId), parameters));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndIdentifierWhenMethodIsSystemAndParametersAreByName()
        {
            var client = new TestJsonRpcClient();
            var parameters = new Dictionary<string, object> { ["a"] = 1L, ["b"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("rpc.m", default(JsonRpcId), parameters));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndErrorDataWhenMethodIsSystem()
        {
            var client = new TestJsonRpcClient();

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long, long>("rpc.m"));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndErrorDataWhenMethodIsSystemAndParametersAreByPosition()
        {
            var client = new TestJsonRpcClient();
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long, long>("rpc.m", parameters));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndErrorDataWhenMethodIsSystemAndParametersAreByName()
        {
            var client = new TestJsonRpcClient();
            var parameters = new Dictionary<string, object> { ["a"] = 1L, ["b"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long, long>("rpc.m", default(JsonRpcId), parameters));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndErrorDataAndIdentifierWhenMethodIsSystem()
        {
            var client = new TestJsonRpcClient();

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long, long>("rpc.m", default(JsonRpcId)));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndErrorDataAndIdentifierWhenMethodIsSystemAndParametersAreByPosition()
        {
            var client = new TestJsonRpcClient();
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long, long>("rpc.m", default(JsonRpcId), parameters));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndErrorDataAndIdentifierWhenMethodIsSystemAndParametersAreByName()
        {
            var client = new TestJsonRpcClient();
            var parameters = new Dictionary<string, object> { ["a"] = 1L, ["b"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long, long>("rpc.m", parameters));
        }

        //###################################################################################################

        [TestMethod]
        public async Task InvokeAsyncWithParametersByPositionWhenParametersIsNull()
        {
            var client = new TestJsonRpcClient();
            var parameters = (object[])null;

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync("m", parameters));
        }

        [TestMethod]
        public async Task InvokeAsyncWithParametersByNameWhenParametersIsNull()
        {
            var client = new TestJsonRpcClient();
            var parameters = (Dictionary<string, object>)null;

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync("m", parameters));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndParametersByPositionWhenParametersIsNull()
        {
            var client = new TestJsonRpcClient();
            var parameters = (object[])null;

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>("m", parameters));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndParametersByNameWhenParametersIsNull()
        {
            var client = new TestJsonRpcClient();
            var parameters = (Dictionary<string, object>)null;

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>("m", parameters));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndIdentifierAndParametersByPositionWhenParametersIsNull()
        {
            var client = new TestJsonRpcClient();
            var parameters = (object[])null;

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>("m", 0L, parameters));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndIdentifierAndParametersByNameWhenParametersIsNull()
        {
            var client = new TestJsonRpcClient();
            var parameters = (Dictionary<string, object>)null;

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>("m", 0L, parameters));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndErrorDataAndIdentifierAndParametersByPositionWhenParametersIsNull()
        {
            var client = new TestJsonRpcClient();
            var parameters = (object[])null;

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long, long>("m", 0L, parameters));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndErrorDataAndIdentifierAndParametersByNameWhenParametersIsNull()
        {
            var client = new TestJsonRpcClient();
            var parameters = (Dictionary<string, object>)null;

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long, long>("m", 0L, parameters));
        }

        //###################################################################################################

        [TestMethod]
        public async Task InvokeAsyncWithResultWhenIdentifierIsEmpty()
        {
            var client = new TestJsonRpcClient();

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("m", default(JsonRpcId)));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultWhenIdentifierIsEmptyAndParametersAreByPosition()
        {
            var client = new TestJsonRpcClient();
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("m", default(JsonRpcId), parameters));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultWhenIdentifierIsEmptyAndParametersAreByName()
        {
            var client = new TestJsonRpcClient();
            var parameters = new Dictionary<string, object> { ["a"] = 1L, ["b"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("m", default(JsonRpcId), parameters));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndErrorDataWhenIdentifierIsEmpty()
        {
            var client = new TestJsonRpcClient();

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long, long>("m", default(JsonRpcId)));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndErrorDataWhenIdentifierIsEmptyAndParametersAreByPosition()
        {
            var client = new TestJsonRpcClient();
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long, long>("m", default(JsonRpcId), parameters));
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndErrorDataWhenIdentifierIsEmptyAndParametersAreByName()
        {
            var client = new TestJsonRpcClient();
            var parameters = new Dictionary<string, object> { ["a"] = 1L, ["b"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long, long>("m", default(JsonRpcId), parameters));
        }

        //###################################################################################################

        [TestMethod]
        public async Task InvokeAsyncWhenCancellationTokenIsCancelled()
        {
            var client = new TestJsonRpcClient();
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync("m", cancellationTokenSource.Token));

            cancellationTokenSource.Dispose();
        }

        [TestMethod]
        public async Task InvokeAsyncWhenCancellationTokenIsCancelledAndParametersAreByPosition()
        {
            var client = new TestJsonRpcClient();
            var parameters = new object[] { 1L, 2L };
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync("m", parameters, cancellationTokenSource.Token));

            cancellationTokenSource.Dispose();
        }

        [TestMethod]
        public async Task InvokeAsyncWhenCancellationTokenIsCancelledAndParametersAreByName()
        {
            var client = new TestJsonRpcClient();
            var parameters = new Dictionary<string, object> { ["a"] = 1L, ["b"] = 2L };
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync("m", parameters, cancellationTokenSource.Token));

            cancellationTokenSource.Dispose();
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultWhenCancellationTokenIsCancelled()
        {
            var client = new TestJsonRpcClient();
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long>("m", cancellationTokenSource.Token));

            cancellationTokenSource.Dispose();
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultWhenCancellationTokenIsCancelledAndParametersAreByPosition()
        {
            var client = new TestJsonRpcClient();
            var parameters = new object[] { 1L, 2L };
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long>("m", parameters, cancellationTokenSource.Token));

            cancellationTokenSource.Dispose();
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultWhenCancellationTokenIsCancelledAndParametersAreByName()
        {
            var client = new TestJsonRpcClient();
            var parameters = new Dictionary<string, object> { ["a"] = 1L, ["b"] = 2L };
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long>("m", parameters, cancellationTokenSource.Token));

            cancellationTokenSource.Dispose();
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndIdentifierWhenCancellationTokenIsCancelled()
        {
            var client = new TestJsonRpcClient();
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long>("m", 0L, cancellationTokenSource.Token));

            cancellationTokenSource.Dispose();
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndIdentifierWhenCancellationTokenIsCancelledAndParametersAreByPosition()
        {
            var client = new TestJsonRpcClient();
            var parameters = new object[] { 1L, 2L };
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long>("m", 0L, parameters, cancellationTokenSource.Token));

            cancellationTokenSource.Dispose();
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndIdentifierWhenCancellationTokenIsCancelledAndParametersAreByName()
        {
            var client = new TestJsonRpcClient();
            var parameters = new Dictionary<string, object> { ["a"] = 1L, ["b"] = 2L };
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long>("m", 0L, parameters, cancellationTokenSource.Token));

            cancellationTokenSource.Dispose();
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndErrorDataWhenCancellationTokenIsCancelled()
        {
            var client = new TestJsonRpcClient();
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long, long>("m", cancellationTokenSource.Token));

            cancellationTokenSource.Dispose();
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndErrorDataWhenCancellationTokenIsCancelledAndParametersAreByPosition()
        {
            var client = new TestJsonRpcClient();
            var parameters = new object[] { 1L, 2L };
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long, long>("m", parameters, cancellationTokenSource.Token));

            cancellationTokenSource.Dispose();
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndErrorDataWhenCancellationTokenIsCancelledAndParametersAreByName()
        {
            var client = new TestJsonRpcClient();
            var parameters = new Dictionary<string, object> { ["a"] = 1L, ["b"] = 2L };
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long, long>("m", parameters, cancellationTokenSource.Token));

            cancellationTokenSource.Dispose();
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndErrorDataAndIdentifierWhenCancellationTokenIsCancelled()
        {
            var client = new TestJsonRpcClient();
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long, long>("m", 0L, cancellationTokenSource.Token));

            cancellationTokenSource.Dispose();
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndErrorDataAndIdentifierWhenCancellationTokenIsCancelledAndParametersAreByPosition()
        {
            var client = new TestJsonRpcClient();
            var parameters = new object[] { 1L, 2L };
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long, long>("m", 0L, parameters, cancellationTokenSource.Token));

            cancellationTokenSource.Dispose();
        }

        [TestMethod]
        public async Task InvokeAsyncWithResultAndErrorDataAndIdentifierWhenCancellationTokenIsCancelledAndParametersAreByName()
        {
            var client = new TestJsonRpcClient();
            var parameters = new Dictionary<string, object> { ["a"] = 1L, ["b"] = 2L };
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long, long>("m", 0L, parameters, cancellationTokenSource.Token));

            cancellationTokenSource.Dispose();
        }

        //###################################################################################################

        [TestMethod]
        public async Task InvokeAsyncWhenHttpStatusCodeIsInvalid()
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
                var exception = await Assert.ThrowsExceptionAsync<JsonRpcProtocolException>(() =>
                    client.InvokeAsync("m"));

                Assert.AreEqual(HttpStatusCode.BadRequest, exception.HttpStatusCode);
                Assert.AreEqual(default, exception.RequestId);
            }
        }

        [TestMethod]
        public async Task InvokeAsyncWhenHttpContentTypeIsEmpty()
        {
            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var content = new ByteArrayContent(Array.Empty<byte>());

                content.Headers.ContentType = null;

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                };

                return Task.FromResult(message);
            });

            using (var client = new TestJsonRpcClient(handler))
            {
                var exception = await Assert.ThrowsExceptionAsync<JsonRpcProtocolException>(() =>
                    client.InvokeAsync<long>("m"));

                Assert.AreEqual(HttpStatusCode.OK, exception.HttpStatusCode);
                Assert.AreEqual(default, exception.RequestId);
            }
        }

        [TestMethod]
        public async Task InvokeAsyncWhenHttpContentTypeIsInvalid()
        {
            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var content = new ByteArrayContent(Array.Empty<byte>());

                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                };

                return Task.FromResult(message);
            });

            using (var client = new TestJsonRpcClient(handler))
            {
                var exception = await Assert.ThrowsExceptionAsync<JsonRpcProtocolException>(() =>
                    client.InvokeAsync<long>("m"));

                Assert.AreEqual(HttpStatusCode.OK, exception.HttpStatusCode);
                Assert.AreEqual(default, exception.RequestId);
            }
        }

        [TestMethod]
        public async Task InvokeAsyncWhenResponseIsBatch()
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

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                };

                return Task.FromResult(message);
            });

            using (var client = new TestJsonRpcClient(handler))
            {
                var exception = await Assert.ThrowsExceptionAsync<JsonRpcProtocolException>(() =>
                    client.InvokeAsync<long>("m"));

                Assert.AreEqual(HttpStatusCode.OK, exception.HttpStatusCode);
                Assert.AreNotEqual(default, exception.RequestId);
            }
        }

        [TestMethod]
        public async Task InvokeAsyncWhenResponseIdIsInvalid()
        {
            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var contentBytes = Encoding.UTF8.GetBytes(EmbeddedResourceManager.GetString("Assets.result_true_id_invalid.json"));
                var content = new ByteArrayContent(contentBytes);

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                };

                return Task.FromResult(message);
            });

            using (var client = new TestJsonRpcClient(handler))
            {
                var exception = await Assert.ThrowsExceptionAsync<JsonRpcClientException>(() =>
                    client.InvokeAsync<long>("m"));

                Assert.AreNotEqual(default, exception.RequestId);
            }
        }

        [TestMethod]
        public async Task InvokeAsyncWhenResultTypeIsInvalid()
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

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                };

                return message;
            });

            using (var client = new TestJsonRpcClient(handler))
            {
                var exception = await Assert.ThrowsExceptionAsync<JsonRpcClientException>(() =>
                    client.InvokeAsync<long>("m"));

                Assert.AreNotEqual(default, exception.RequestId);
            }
        }

        [TestMethod]
        public async Task InvokeAsyncWhenResponseContractIsInvalidAndResponseIsNotExpected()
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

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                };

                return message;
            });

            using (var client = new TestJsonRpcClient(handler))
            {
                var exception = await Assert.ThrowsExceptionAsync<JsonRpcProtocolException>(() =>
                    client.InvokeAsync("m"));

                Assert.AreEqual(HttpStatusCode.OK, exception.HttpStatusCode);
                Assert.AreEqual(default, exception.RequestId);
            }
        }

        [TestMethod]
        public async Task InvokeAsyncWhenResponseContractIsInvalidAndResponseIsExpected()
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
                var exception = await Assert.ThrowsExceptionAsync<JsonRpcProtocolException>(() =>
                    client.InvokeAsync<long>("m"));

                Assert.AreEqual(HttpStatusCode.NoContent, exception.HttpStatusCode);
                Assert.AreNotEqual(default, exception.RequestId);
            }
        }

        [TestMethod]
        public async Task InvokeAsyncWhenResponseIsNotExpected()
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
        public async Task InvokeAsyncWhenResponseIsExpected()
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
        public async Task InvokeAsyncWhenResponseWithErrorIsExpected()
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
        public async Task InvokeAsyncWithCustomRequestHeader()
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

        [TestMethod]
        public async Task InvokeAsyncWithBrotliEncodedResponse()
        {
            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)(async (request) =>
            {
                var requestString = await request.Content.ReadAsStringAsync();
                var requestObject = JObject.Parse(requestString);
                var responseObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.result_true_success_true.json"));

                responseObject["id"] = requestObject["id"];

                var contentBytes = Encoding.UTF8.GetBytes(responseObject.ToString());
                var content = new ByteArrayContent(CompressWithBrotli(contentBytes));

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Headers.ContentEncoding.Add("br");

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

                Assert.AreEqual(1L, result);
            }
        }
    }
}