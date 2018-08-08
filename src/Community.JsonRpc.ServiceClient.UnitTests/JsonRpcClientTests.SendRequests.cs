using System;
using System.Data.JsonRpc;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Community.JsonRpc.ServiceClient.UnitTests.TestStubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Community.JsonRpc.ServiceClient.UnitTests
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

        //###################################################################################################

        [TestMethod]
        public async Task SendJsonRpcRequestsAsyncWithCustomHttpProtocolVersion()
        {
            var requests = new[]
            {
                new JsonRpcRequest("m"),
                new JsonRpcRequest("m")
            };

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

                await client.PublicSendJsonRpcRequestsAsync(requests, default);
            }
        }

        [TestMethod]
        public async Task SendJsonRpcRequestsAsyncWithCustomRequestHeader()
        {
            var requests = new[]
            {
                new JsonRpcRequest("m"),
                new JsonRpcRequest("m")
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
                client.VisitHttpRequestHeadersAction = headers => headers.Authorization = authorizationHeader;

                await client.PublicSendJsonRpcRequestsAsync(requests, default);
            }
        }

        [TestMethod]
        public async Task SendJsonRpcRequestsAsyncWithBrotliEncodedResponse()
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
                var content = new ByteArrayContent(CompressWithBrotli(contentBytes));

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
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
                client.PublicContractResolver.AddResponseBinding(0L, "m");
                client.PublicContractResolver.AddResponseBinding(1L, "m");

                var responses = await client.PublicSendJsonRpcRequestsAsync(requests, default);

                Assert.IsNotNull(responses);
                Assert.AreEqual(2, responses.Count);
            }
        }
    }
}