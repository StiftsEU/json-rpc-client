using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Community.JsonRpc.ServiceClient.Tests.Internal;
using Community.JsonRpc.ServiceClient.Tests.Resources;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Community.JsonRpc.ServiceClient.Tests
{
    public sealed class JsonRpcClientTests
    {
        private readonly ITestOutputHelper _output;

        public JsonRpcClientTests(ITestOutputHelper output)
        {
            _output = output;
        }

        private HttpClient CreateEmptyHttpMessageInvoker()
        {
            return new HttpClient(new TestHttpMessageHandler());
        }

        [Fact]
        public void ConstructorWhenServiceUriIsStringAndIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcClient((string)null, CreateEmptyHttpMessageInvoker()));
        }

        [Fact]
        public void ConstructorWhenServiceUriIsStringAndIsRelative()
        {
            Assert.Throws<UriFormatException>(() =>
                new JsonRpcClient("/api", CreateEmptyHttpMessageInvoker()));
        }

        [Fact]
        public void ConstructorWhenServiceUriIsUriAndIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonRpcClient((Uri)null, CreateEmptyHttpMessageInvoker()));
        }

        [Fact]
        public void ConstructorWhenServiceUriIssUriAndIsRelative()
        {
            Assert.Throws<ArgumentException>(() =>
                new JsonRpcClient(new Uri("/api", UriKind.Relative), CreateEmptyHttpMessageInvoker()));
        }

        [Fact]
        public async void InvokeWhenMethodIsNull()
        {
            var client = new JsonRpcClient("https://localhost", CreateEmptyHttpMessageInvoker());

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>((string)null));
        }

        [Fact]
        public async void InvokeWhenMethodIsNullAndParametersAreByPosition()
        {
            var client = new JsonRpcClient("https://localhost", CreateEmptyHttpMessageInvoker());

            var parameters = new object[]
            {
                1L,
                2L
            };

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>((string)null, parameters));
        }

        [Fact]
        public async void InvokeWhenMethodIsNullAndParametersAreByName()
        {
            var client = new JsonRpcClient("https://localhost", CreateEmptyHttpMessageInvoker());

            var parameters = new Dictionary<string, object>
            {
                ["p1"] = 1L,
                ["p2"] = 2L
            };

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>((string)null, parameters));
        }

        [Fact]
        public async void InvokeWhenMethodIsSystem()
        {
            var client = new JsonRpcClient("https://localhost", CreateEmptyHttpMessageInvoker());

            await Assert.ThrowsAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("rpc.m"));
        }

        [Fact]
        public async void InvokeWhenMethodIsSystemAndParametersAreByPosition()
        {
            var client = new JsonRpcClient("https://localhost", CreateEmptyHttpMessageInvoker());

            var parameters = new object[]
            {
                1L,
                2L
            };

            await Assert.ThrowsAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("rpc.m", parameters));
        }

        [Fact]
        public async void InvokeWhenMethodIsSystemAndParametersAreByName()
        {
            var client = new JsonRpcClient("https://localhost", CreateEmptyHttpMessageInvoker());

            var parameters = new Dictionary<string, object>
            {
                ["p1"] = 1L,
                ["p2"] = 2L
            };

            await Assert.ThrowsAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("rpc.m", parameters));
        }

        [Fact]
        public async void InvokeWhenParametersAreByPositionAndIsNull()
        {
            var client = new JsonRpcClient("https://localhost", CreateEmptyHttpMessageInvoker());
            var parameters = (object[])null;

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>("m", parameters));
        }

        [Fact]
        public async void InvokeWhenParametersAreByPositionAndIsEmpty()
        {
            var client = new JsonRpcClient("https://localhost", CreateEmptyHttpMessageInvoker());
            var parameters = new object[] { };

            await Assert.ThrowsAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("m", parameters));
        }

        [Fact]
        public async void InvokeWhenParametersAreByNameAndIsNull()
        {
            var client = new JsonRpcClient("https://localhost", CreateEmptyHttpMessageInvoker());
            var parameters = (Dictionary<string, object>)null;

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>("m", parameters));
        }

        [Fact]
        public async void InvokeWhenParametersAreByNameAndIsEmpty()
        {
            var client = new JsonRpcClient("https://localhost", CreateEmptyHttpMessageInvoker());
            var parameters = new Dictionary<string, object>();

            await Assert.ThrowsAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("m", parameters));
        }

        [Fact]
        public async void InvokeWhenCancellationTokenIsCancelled()
        {
            var client = new JsonRpcClient("https://localhost", CreateEmptyHttpMessageInvoker());
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long>("m", cancellationTokenSource.Token));
        }

        [Fact]
        public async void InvokeWhenCancellationTokenIsCancelledAndParametersAreByPosition()
        {
            var client = new JsonRpcClient("https://localhost", CreateEmptyHttpMessageInvoker());

            var parameters = new object[]
            {
                1L,
                2L
            };

            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long>("m", parameters, cancellationTokenSource.Token));
        }

        [Fact]
        public async void InvokeWhenCancellationTokenIsCancelledAndParametersAreByName()
        {
            var client = new JsonRpcClient("https://localhost", CreateEmptyHttpMessageInvoker());

            var parameters = new Dictionary<string, object>
            {
                ["p1"] = 1L,
                ["p2"] = 2L
            };

            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long>("m", parameters, cancellationTokenSource.Token));
        }

        [Fact]
        public async void InvokeWhenHttpStatusCodeIsInvalid()
        {
            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                };

                return Task.FromResult(message);
            });

            using (var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpMessageHandler(_output, handler))))
            {
                var exception = await Assert.ThrowsAsync<JsonRpcRequestException>(() =>
                    client.InvokeAsync<long>("m"));

                Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
            }
        }

        [Fact]
        public async void InvokeWhenHttpContentTypeIsInvalid()
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

            using (var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpMessageHandler(_output, handler))))
            {
                var exception = await Assert.ThrowsAsync<JsonRpcRequestException>(() =>
                    client.InvokeAsync<long>("m"));

                Assert.Equal(HttpStatusCode.OK, exception.StatusCode);
            }
        }

        [Fact]
        public async void InvokeWhenHttpContentLengthIsEmpty()
        {
            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var content = new ByteArrayContent(Array.Empty<byte>());

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Headers.ContentLength = null;

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                };

                return Task.FromResult(message);
            });

            using (var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpMessageHandler(_output, handler))))
            {
                var exception = await Assert.ThrowsAsync<JsonRpcRequestException>(() =>
                    client.InvokeAsync<long>("m"));

                Assert.Equal(HttpStatusCode.OK, exception.StatusCode);
            }
        }

        [Fact]
        public async void InvokeWhenHttpContentLengthIsInvalid()
        {
            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var content = new ByteArrayContent(Array.Empty<byte>());

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Headers.ContentLength = 32L;

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                };

                return Task.FromResult(message);
            });

            using (var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpMessageHandler(_output, handler))))
            {
                var exception = await Assert.ThrowsAsync<JsonRpcRequestException>(() =>
                    client.InvokeAsync<long>("m"));

                Assert.Equal(HttpStatusCode.OK, exception.StatusCode);
            }
        }

        [Fact]
        public async void InvokeWhenResponseIsBatch()
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

            using (var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpMessageHandler(_output, handler))))
            {
                await Assert.ThrowsAsync<JsonRpcContractException>(() =>
                    client.InvokeAsync<long>("m"));
            }
        }


        [Fact]
        public async void InvokeWhenResponseIdIsInvalid()
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

            using (var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpMessageHandler(_output, handler))))
            {
                await Assert.ThrowsAsync<JsonRpcContractException>(() =>
                    client.InvokeAsync<long>("m"));
            }
        }

        [Fact]
        public async void InvokeWhenResultTypeIsInvalid()
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

            using (var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpMessageHandler(_output, handler))))
            {
                // NOTE: JsonRpcContractException should be expected after serializer update

                await Assert.ThrowsAnyAsync<Exception>(() =>
                    client.InvokeAsync<long>("m"));
            }
        }

        [Fact]
        public async void InvokeWhenResponseContractIsInvalidAndResponseIsNotExpected()
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

            using (var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpMessageHandler(_output, handler))))
            {
                var exception = await Assert.ThrowsAsync<JsonRpcContractException>(() =>
                    client.InvokeAsync<VoidValue>("m"));

                Assert.Equal(string.Empty, exception.RequestId);
            }
        }

        [Fact]
        public async void InvokeWhenResponseContractIsInvalidAndResponseIsExpected()
        {
            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NoContent
                };

                return Task.FromResult(message);
            });

            using (var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpMessageHandler(_output, handler))))
            {
                var exception = await Assert.ThrowsAsync<JsonRpcContractException>(() =>
                    client.InvokeAsync<long>("m"));

                Assert.NotEqual(string.Empty, exception.RequestId);
            }
        }

        [Fact]
        public async void InvokeWhenResponseIsNotExpected()
        {
            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NoContent
                };

                return Task.FromResult(message);
            });

            using (var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpMessageHandler(_output, handler))))
            {
                await client.InvokeAsync<VoidValue>("m");
            }
        }

        [Fact]
        public async void InvokeWhenResponseIsExpected()
        {
            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)(async (request) =>
            {
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

            using (var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpMessageHandler(_output, handler))))
            {
                var result = await client.InvokeAsync<long>("m");

                Assert.Equal(1L, result);
            }
        }

        [Fact]
        public async void InvokeWhenResponseWithErrorIsExpected()
        {
            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)(async (request) =>
            {
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

            using (var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpMessageHandler(_output, handler))))
            {
                var exception = await Assert.ThrowsAsync<JsonRpcServiceException>(() =>
                    client.InvokeAsync<long>("m"));

                Assert.Equal(1L, exception.Code);
                Assert.Equal("e", exception.Message);
            }
        }
    }
}