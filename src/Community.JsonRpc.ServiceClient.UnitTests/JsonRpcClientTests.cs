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
using Community.JsonRpc.ServiceClient.Tests.Internal;
using Community.JsonRpc.ServiceClient.Tests.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Community.JsonRpc.ServiceClient.Tests
{
    [TestClass]
    public sealed class JsonRpcClientTests
    {
        private HttpClient CreateEmptyHttpInvoker()
        {
            return new HttpClient(new TestHttpHandler());
        }

        [TestMethod]
        public void ConstructorWhenServiceUriIsStringAndIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcClient((string)null, CreateEmptyHttpInvoker()));
        }

        [TestMethod]
        public void ConstructorWhenServiceUriIsStringAndIsRelative()
        {
            Assert.ThrowsException<UriFormatException>(() =>
                new JsonRpcClient("/api", CreateEmptyHttpInvoker()));
        }

        [TestMethod]
        public void ConstructorWhenServiceUriIsStringAndInvokerIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcClient("https://localhost", null));
        }

        [TestMethod]
        public void ConstructorWhenServiceUriIsUriAndIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcClient((Uri)null, CreateEmptyHttpInvoker()));
        }

        [TestMethod]
        public void ConstructorWhenServiceUriIsUriAndIsRelative()
        {
            Assert.ThrowsException<ArgumentException>(() =>
                new JsonRpcClient(new Uri("/api", UriKind.Relative), CreateEmptyHttpInvoker()));
        }

        [TestMethod]
        public void ConstructorWhenServiceUriIsUriAndInvokerIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcClient(new Uri("https://localhost", UriKind.Absolute), null));
        }

        [TestMethod]
        public async Task InvokeWhenMethodIsNull()
        {
            var client = new JsonRpcClient("https://localhost", CreateEmptyHttpInvoker());

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>((string)null));
        }

        [TestMethod]
        public async Task InvokeWhenIdentifierIsEmpty()
        {
            var client = new JsonRpcClient("https://localhost", CreateEmptyHttpInvoker());

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("m", new JsonRpcId()));
        }

        [TestMethod]
        public async Task InvokeWhenMethodIsNullAndParametersAreByPosition()
        {
            var client = new JsonRpcClient("https://localhost", CreateEmptyHttpInvoker());

            var parameters = new object[]
            {
                1L,
                2L
            };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>((string)null, parameters));
        }

        [TestMethod]
        public async Task InvokeWhenIdentifierIsEmptyAndParametersAreByPosition()
        {
            var client = new JsonRpcClient("https://localhost", CreateEmptyHttpInvoker());

            var parameters = new object[]
            {
               1L,
               2L
            };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("m", new JsonRpcId(), parameters));
        }

        [TestMethod]
        public async Task InvokeWhenMethodIsNullAndParametersAreByName()
        {
            var client = new JsonRpcClient("https://localhost", CreateEmptyHttpInvoker());

            var parameters = new Dictionary<string, object>
            {
                ["p1"] = 1L,
                ["p2"] = 2L
            };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>((string)null, parameters));
        }

        [TestMethod]
        public async Task InvokeWhenIdentifierIsEmptyAndParametersAreByName()
        {
            var client = new JsonRpcClient("https://localhost", CreateEmptyHttpInvoker());

            var parameters = new Dictionary<string, object>
            {
                ["p1"] = 1L,
                ["p2"] = 2L
            };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("m", new JsonRpcId(), parameters));
        }

        [TestMethod]
        public async Task InvokeWhenMethodIsSystem()
        {
            var client = new JsonRpcClient("https://localhost", CreateEmptyHttpInvoker());

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("rpc.m"));
        }

        [TestMethod]
        public async Task InvokeWhenMethodIsSystemAndParametersAreByPosition()
        {
            var client = new JsonRpcClient("https://localhost", CreateEmptyHttpInvoker());

            var parameters = new object[]
            {
                1L,
                2L
            };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("rpc.m", parameters));
        }

        [TestMethod]
        public async Task InvokeWhenMethodIsSystemAndParametersAreByName()
        {
            var client = new JsonRpcClient("https://localhost", CreateEmptyHttpInvoker());

            var parameters = new Dictionary<string, object>
            {
                ["p1"] = 1L,
                ["p2"] = 2L
            };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("rpc.m", parameters));
        }

        [TestMethod]
        public async Task InvokeWhenParametersAreByPositionAndIsNull()
        {
            var client = new JsonRpcClient("https://localhost", CreateEmptyHttpInvoker());
            var parameters = (object[])null;

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>("m", parameters));
        }

        [TestMethod]
        public async Task InvokeWhenParametersAreByNameAndIsNull()
        {
            var client = new JsonRpcClient("https://localhost", CreateEmptyHttpInvoker());
            var parameters = (Dictionary<string, object>)null;

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>("m", parameters));
        }

        [TestMethod]
        public async Task InvokeWhenCancellationTokenIsCancelled()
        {
            var client = new JsonRpcClient("https://localhost", CreateEmptyHttpInvoker());
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long>("m", cancellationTokenSource.Token));
        }

        [TestMethod]
        public async Task InvokeWhenCancellationTokenIsCancelledAndParametersAreByPosition()
        {
            var client = new JsonRpcClient("https://localhost", CreateEmptyHttpInvoker());

            var parameters = new object[]
            {
                1L,
                2L
            };

            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long>("m", parameters, cancellationTokenSource.Token));
        }

        [TestMethod]
        public async Task InvokeWhenCancellationTokenIsCancelledAndParametersAreByName()
        {
            var client = new JsonRpcClient("https://localhost", CreateEmptyHttpInvoker());

            var parameters = new Dictionary<string, object>
            {
                ["p1"] = 1L,
                ["p2"] = 2L
            };

            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long>("m", parameters, cancellationTokenSource.Token));
        }

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

            using (var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler(handler))))
            {
                var exception = await Assert.ThrowsExceptionAsync<JsonRpcRequestException>(() =>
                    client.InvokeAsync<long>("m"));

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

            using (var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler(handler))))
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

            using (var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler(handler))))
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
                var contentBytes = Encoding.UTF8.GetBytes(EmbeddedResourceManager.GetString("Assets.batch_true.json"));
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

            using (var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler(handler))))
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

            using (var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler(handler))))
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

            using (var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler(handler))))
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

            using (var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler(handler))))
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

            using (var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler(handler))))
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

            using (var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler(handler))))
            {
                await client.InvokeAsync("m");

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

            using (var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler(handler))))
            {
                var result = await client.InvokeAsync<long>("m");

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

            using (var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler(handler))))
            {
                var exception = await Assert.ThrowsExceptionAsync<JsonRpcServiceException>(() =>
                    client.InvokeAsync<long>("m"));

                CollectionAssert.Contains(requestAcceptHeader.ToArray(), new MediaTypeWithQualityHeaderValue("application/json"));
                Assert.AreEqual(1L, exception.Code);
                Assert.AreEqual("e", exception.Message);
            }
        }

        [TestMethod]
        public async Task InvokeWhenHttpVersionIsSpecified()
        {
            var httpVersion = new Version(2, 0);

            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                Assert.AreEqual(httpVersion, request.Version);

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NoContent
                };

                return Task.FromResult(message);
            });

            var httpClient = new HttpClient(new TestHttpHandler(handler));

            using (var client = new TestJsonRpcClientWithHttpVersion("https://localhost", httpClient, httpVersion))
            {
                await client.InvokeAsync("m");
            }
        }

        [TestMethod]
        public async Task InvokeWithAuthorizationHeader()
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

            var httpClient = new HttpClient(new TestHttpHandler(handler));

            using (var client = new TestJsonRpcClientWithAuthorizationHeader("https://localhost", httpClient, authorizationHeader))
            {
                await client.InvokeAsync("m");
            }
        }
    }
}