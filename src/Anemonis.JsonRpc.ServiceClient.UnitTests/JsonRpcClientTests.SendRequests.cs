using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Anemonis.JsonRpc.ServiceClient.UnitTests.Resources;
using Anemonis.JsonRpc.ServiceClient.UnitTests.TestStubs;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json.Linq;

#pragma warning disable IDE0039

namespace Anemonis.JsonRpc.ServiceClient.UnitTests
{
    public partial class JsonRpcClientTests
    {
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
                new JsonRpcRequest(1L, "m"),
                new JsonRpcRequest(1L, "m")
            };

            using (var client = new TestJsonRpcClient())
            {
                var exception = await Assert.ThrowsExceptionAsync<JsonRpcClientException>(() =>
                     client.PublicSendJsonRpcRequestsAsync(requests, default));

                Assert.AreEqual(default, exception.RequestId);
            }
        }

        [TestMethod]
        public async Task SendJsonRpcRequestsAsyncWhenHttpStatusCodeIsInvalid()
        {
            var requests = new[]
            {
                new JsonRpcRequest(1L, "m"),
                new JsonRpcRequest(2L, "m")
            };

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
                client.PublicContractResolver.AddResponseContract("m", new JsonRpcResponseContract(typeof(string)));
                client.PublicContractResolver.AddResponseBinding(1L, "m");
                client.PublicContractResolver.AddResponseBinding(2L, "m");

                var exception = await Assert.ThrowsExceptionAsync<JsonRpcProtocolException>(() =>
                     client.PublicSendJsonRpcRequestsAsync(requests, default));

                Assert.AreEqual(HttpStatusCode.BadRequest, exception.HttpStatusCode);
                Assert.AreEqual(default, exception.RequestId);
            }
        }

        [TestMethod]
        public async Task SendJsonRpcRequestsAsyncWhenHttpContentTypeIsEmpty()
        {
            var requests = new[]
            {
                new JsonRpcRequest(1L, "m"),
                new JsonRpcRequest(2L, "m")
            };

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
                     client.PublicSendJsonRpcRequestsAsync(requests, default));

                Assert.AreEqual(HttpStatusCode.OK, exception.HttpStatusCode);
                Assert.AreEqual(default, exception.RequestId);
            }
        }

        [TestMethod]
        public async Task SendJsonRpcRequestsAsyncWhenHttpContentTypeIsInvalid()
        {
            var requests = new[]
            {
                new JsonRpcRequest(1L, "m"),
                new JsonRpcRequest(2L, "m")
            };

            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var content = new ByteArrayContent(Array.Empty<byte>());

                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");

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
                     client.PublicSendJsonRpcRequestsAsync(requests, default));

                Assert.AreEqual(HttpStatusCode.OK, exception.HttpStatusCode);
                Assert.AreEqual(default, exception.RequestId);
            }
        }

        [TestMethod]
        public async Task SendJsonRpcRequestsAsyncWhenResponsesDoNotMatch()
        {
            var requests = new[]
            {
                new JsonRpcRequest(1L, "m"),
                new JsonRpcRequest(2L, "m")
            };

            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var responseObject = new JObject
                {
                    ["jsonrpc"] = "2.0",
                    ["id"] = 1L,
                    ["result"] = null
                };

                var responseBatch = new JArray(responseObject);
                var contentBytes = Encoding.UTF8.GetBytes(responseBatch.ToString());
                var content = new ByteArrayContent(contentBytes);

                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

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
                client.PublicContractResolver.AddResponseBinding(1L, "m");
                client.PublicContractResolver.AddResponseBinding(2L, "m");

                var exception = await Assert.ThrowsExceptionAsync<JsonRpcProtocolException>(() =>
                     client.PublicSendJsonRpcRequestsAsync(requests, default));

                Assert.AreEqual(HttpStatusCode.OK, exception.HttpStatusCode);
                Assert.AreEqual(default, exception.RequestId);
            }
        }

        [TestMethod]
        public async Task SendJsonRpcRequestsAsyncWhenResponsesHaveDuplicateIdentifiers()
        {
            var requests = new[]
            {
                new JsonRpcRequest(1L, "m"),
                new JsonRpcRequest(2L, "m")
            };

            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var responseObject0 = new JObject
                {
                    ["jsonrpc"] = "2.0",
                    ["id"] = 1L,
                    ["result"] = null
                };

                var responseObject1 = new JObject
                {
                    ["jsonrpc"] = "2.0",
                    ["id"] = 1L,
                    ["result"] = null
                };

                var responseBatch = new JArray(responseObject0, responseObject1);
                var contentBytes = Encoding.UTF8.GetBytes(responseBatch.ToString());
                var content = new ByteArrayContent(contentBytes);

                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

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
                client.PublicContractResolver.AddResponseBinding(1L, "m");
                client.PublicContractResolver.AddResponseBinding(2L, "m");

                var exception = await Assert.ThrowsExceptionAsync<JsonRpcProtocolException>(() =>
                     client.PublicSendJsonRpcRequestsAsync(requests, default));

                Assert.AreEqual(HttpStatusCode.OK, exception.HttpStatusCode);
                Assert.AreEqual(default, exception.RequestId);
            }
        }

        [TestMethod]
        public async Task SendJsonRpcRequestsAsyncWhenResponseIsNotBatchAndInvalid()
        {
            var requests = new[]
            {
                new JsonRpcRequest(1L, "m")
            };

            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var responseObject = new JObject
                {
                    ["jsonrpc"] = "0.0",
                    ["id"] = 1L,
                    ["result"] = null
                };

                var contentBytes = Encoding.UTF8.GetBytes(responseObject.ToString());
                var content = new ByteArrayContent(contentBytes);

                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

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
                client.PublicContractResolver.AddResponseBinding(1L, "m");

                var exception = await Assert.ThrowsExceptionAsync<JsonRpcClientException>(() =>
                     client.PublicSendJsonRpcRequestsAsync(requests, default));

                Assert.AreEqual(default, exception.RequestId);
            }
        }

        [TestMethod]
        public async Task SendJsonRpcRequestsAsyncWhenResponseIsNotBatchAndHasValidResult()
        {
            var requests = new[]
            {
                new JsonRpcRequest(1L, "m")
            };

            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var responseObject = new JObject
                {
                    ["jsonrpc"] = "2.0",
                    ["id"] = 1L,
                    ["result"] = null
                };

                var contentBytes = Encoding.UTF8.GetBytes(responseObject.ToString());
                var content = new ByteArrayContent(contentBytes);

                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

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
                client.PublicContractResolver.AddResponseBinding(1L, "m");

                var exception = await Assert.ThrowsExceptionAsync<JsonRpcProtocolException>(() =>
                     client.PublicSendJsonRpcRequestsAsync(requests, default));

                Assert.AreEqual(HttpStatusCode.OK, exception.HttpStatusCode);
                Assert.AreEqual(default, exception.RequestId);
            }
        }

        [TestMethod]
        public async Task SendJsonRpcRequestsAsyncWhenResponseIsInvalid()
        {
            var requests = new[]
            {
                new JsonRpcRequest(1L, "m"),
                new JsonRpcRequest(2L, "m")
            };

            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var responseObject0 = new JObject
                {
                    ["jsonrpc"] = "2.0",
                    ["id"] = 1L,
                    ["result"] = null
                };

                var responseObject1 = new JObject
                {
                    ["jsonrpc"] = "2.0",
                    ["id"] = 2L
                };

                var responseBatch = new JArray(responseObject0, responseObject1);
                var contentBytes = Encoding.UTF8.GetBytes(responseBatch.ToString());
                var content = new ByteArrayContent(contentBytes);

                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

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
                client.PublicContractResolver.AddResponseBinding(1L, "m");
                client.PublicContractResolver.AddResponseBinding(2L, "m");

                var exception = await Assert.ThrowsExceptionAsync<AggregateException>(() =>
                     client.PublicSendJsonRpcRequestsAsync(requests, default));

                Assert.IsNotNull(exception);
                Assert.IsNotNull(exception.InnerExceptions);
                Assert.AreEqual(1, exception.InnerExceptions.Count);
                Assert.IsInstanceOfType(exception.InnerExceptions[0], typeof(JsonRpcClientException));
            }
        }

        [TestMethod]
        public async Task SendJsonRpcRequestsAsyncWhenRequestsAreNotifications()
        {
            var requests = new[]
            {
                new JsonRpcRequest(default, "m"),
                new JsonRpcRequest(default, "m")
            };

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
                client.PublicContractResolver.AddResponseContract("m", new JsonRpcResponseContract(typeof(string)));
                client.PublicContractResolver.AddResponseBinding(1L, "m");
                client.PublicContractResolver.AddResponseBinding(2L, "m");

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
                new JsonRpcRequest(1L, "m"),
                new JsonRpcRequest(2L, "m")
            };

            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var contentBytes = Encoding.UTF8.GetBytes(EmbeddedResourceManager.GetString("Assets.res_b1i1e0d0.json"));
                var content = new ByteArrayContent(contentBytes);

                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

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
                client.PublicContractResolver.AddResponseBinding(1L, "m");
                client.PublicContractResolver.AddResponseBinding(2L, "m");

                var responses = await client.PublicSendJsonRpcRequestsAsync(requests, default);

                Assert.IsNotNull(responses);
                Assert.AreEqual(2, responses.Count);
            }
        }

        [TestMethod]
        public async Task SendJsonRpcRequestsAsyncWithCustomRequestHeader()
        {
            var requests = new[]
            {
                new JsonRpcRequest(default, "m"),
                new JsonRpcRequest(default, "m")
            };

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
                client.VisitHttpRequestMessageAction = message => message.Headers.Authorization = authorizationHeader;

                await client.PublicSendJsonRpcRequestsAsync(requests, default);
            }
        }

        [TestMethod]
        public async Task SendJsonRpcRequestsAsyncWithBrotliEncodedResponse()
        {
            var requests = new[]
            {
                new JsonRpcRequest(1L, "m"),
                new JsonRpcRequest(2L, "m")
            };

            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var contentBytes = Encoding.UTF8.GetBytes(EmbeddedResourceManager.GetString("Assets.res_b1i1e0d0.json"));
                var content = new ByteArrayContent(CompressionEncoder.Encode(contentBytes, "br"));

                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
                content.Headers.ContentEncoding.Add("br");

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
                client.PublicContractResolver.AddResponseBinding(1L, "m");
                client.PublicContractResolver.AddResponseBinding(2L, "m");

                var responses = await client.PublicSendJsonRpcRequestsAsync(requests, default);

                Assert.IsNotNull(responses);
                Assert.AreEqual(2, responses.Count);
            }
        }
    }
}

#pragma warning restore IDE0034, IDE0039
