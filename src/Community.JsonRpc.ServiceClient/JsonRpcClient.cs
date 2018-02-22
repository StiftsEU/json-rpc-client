using System;
using System.Collections.Generic;
using System.Data.JsonRpc;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Community.JsonRpc.ServiceClient.Internal;
using Community.JsonRpc.ServiceClient.Resources;

namespace Community.JsonRpc.ServiceClient
{
    /// <summary>Represents a JSON-RPC 2.0 service client.</summary>
    public sealed class JsonRpcClient : IDisposable
    {
        private static readonly MediaTypeHeaderValue _mediaTypeHeaderValue = new MediaTypeHeaderValue("application/json");

        private readonly HttpMessageInvoker _httpMessageInvoker;
        private readonly Uri _serviceUri;

        private readonly JsonRpcSerializer _jsonRpcSerializer =
            new JsonRpcSerializer(
                new Dictionary<string, JsonRpcRequestContract>(0),
                new Dictionary<string, JsonRpcResponseContract>(0),
                new Dictionary<JsonRpcId, string>(0),
                new Dictionary<JsonRpcId, JsonRpcResponseContract>(1));

        /// <summary>Initializes a new instance of the <see cref="JsonRpcClient" /> class.</summary>
        /// <param name="serviceUri">The service URI.</param>
        /// <param name="httpMessageInvoker">The component for sending HTTP requests.</param>
        /// <exception cref="ArgumentNullException"><paramref name="serviceUri" /> is <see langword="null" />.</exception>
        /// <exception cref="FormatException"><paramref name="serviceUri" /> contains a relative URI or is not correctly formed.</exception>
        public JsonRpcClient(string serviceUri, HttpMessageInvoker httpMessageInvoker = null)
        {
            if (serviceUri == null)
            {
                throw new ArgumentNullException(nameof(serviceUri));
            }

            _serviceUri = new Uri(serviceUri, UriKind.Absolute);
            _httpMessageInvoker = httpMessageInvoker ?? CreateHttpMessageInvoker();
        }

        /// <summary>Invokes the specified service method.</summary>
        /// <typeparam name="T">The type of the service method result.</typeparam>
        /// <param name="method">The name of the service method.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the service method result.</returns>
        /// <exception cref="ArgumentException"><paramref name="method" /> is a system extension method.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> is <see langword="null" />.</exception>
        public async Task<T> InvokeAsync<T>(string method, CancellationToken cancellationToken = default)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (JsonRpcRequest.IsSystemMethod(method))
            {
                throw new ArgumentException(Strings.GetString("invoke.method.invalid_name"), nameof(method));
            }

            var request = new JsonRpcRequest(method, CreateIdentifier<T>());
            var response = await InvokeAsync(request, CreateContract<T>(), cancellationToken).ConfigureAwait(false);

            return response != null ? (T)response.Result : default;
        }

        /// <summary>Invokes the specified service method.</summary>
        /// <typeparam name="T">The type of the service method result.</typeparam>
        /// <param name="method">The name of the service method.</param>
        /// <param name="parameters">The parameters to be used during the invocation of the service method.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the service method result.</returns>
        /// <exception cref="ArgumentException"><paramref name="method" /> is a system extension method or parameters count equals zero.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> or <paramref name="parameters" /> is <see langword="null" />.</exception>
        public async Task<T> InvokeAsync<T>(string method, IReadOnlyList<object> parameters, CancellationToken cancellationToken = default)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (JsonRpcRequest.IsSystemMethod(method))
            {
                throw new ArgumentException(Strings.GetString("invoke.method.invalid_name"), nameof(method));
            }
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }
            if (parameters.Count == 0)
            {
                throw new ArgumentException(Strings.GetString("invoke.params.invalid_count"), nameof(parameters));
            }

            var request = new JsonRpcRequest(method, CreateIdentifier<T>(), parameters);
            var response = await InvokeAsync(request, CreateContract<T>(), cancellationToken).ConfigureAwait(false);

            return response != null ? (T)response.Result : default;
        }

        /// <summary>Invokes the specified service method.</summary>
        /// <typeparam name="T">The type of the service method result.</typeparam>
        /// <param name="method">The name of the service method.</param>
        /// <param name="parameters">The parameters to be used during the invocation of the service method.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the service method result.</returns>
        /// <exception cref="ArgumentException"><paramref name="method" /> is a system extension method or parameters count equals zero.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> or <paramref name="parameters" /> is <see langword="null" />.</exception>
        public async Task<T> InvokeAsync<T>(string method, IReadOnlyDictionary<string, object> parameters, CancellationToken cancellationToken = default)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (JsonRpcRequest.IsSystemMethod(method))
            {
                throw new ArgumentException(Strings.GetString("invoke.method.invalid_name"), nameof(method));
            }
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }
            if (parameters.Count == 0)
            {
                throw new ArgumentException(Strings.GetString("invoke.params.invalid_count"), nameof(parameters));
            }

            var request = new JsonRpcRequest(method, CreateIdentifier<T>(), parameters);
            var response = await InvokeAsync(request, CreateContract<T>(), cancellationToken).ConfigureAwait(false);

            return response != null ? (T)response.Result : default;
        }

        private async Task<JsonRpcResponse> InvokeAsync(JsonRpcRequest request, JsonRpcResponseContract contract, CancellationToken cancellationToken)
        {
            using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, _serviceUri))
            {
                var httpRequestString = _jsonRpcSerializer.SerializeRequest(request);

                cancellationToken.ThrowIfCancellationRequested();

                var httpRequestContent = new StringContent(httpRequestString);

                httpRequestContent.Headers.ContentType = _mediaTypeHeaderValue;
                httpRequestMessage.Content = httpRequestContent;

                using (var httpResponseMessage = await _httpMessageInvoker.SendAsync(httpRequestMessage, cancellationToken).ConfigureAwait(false))
                {
                    switch (httpResponseMessage.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            {
                                if (contract == null)
                                {
                                    throw new JsonRpcContractException(Strings.GetString("protocol.service.message.invalid_value"));
                                }

                                var contentType = httpResponseMessage.Content.Headers.ContentType;

                                if ((contentType == null) || (string.Compare(contentType.MediaType, _mediaTypeHeaderValue.MediaType, StringComparison.OrdinalIgnoreCase) != 0))
                                {
                                    throw new JsonRpcRequestException(Strings.GetString("protocol.http.headers.invalid_set"), httpResponseMessage.StatusCode);
                                }

                                var contentLength = httpResponseMessage.Content.Headers.ContentLength;

                                if (contentLength == null)
                                {
                                    throw new JsonRpcRequestException(Strings.GetString("protocol.http.headers.invalid_set"), httpResponseMessage.StatusCode);
                                }

                                var httpResponseString = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

                                cancellationToken.ThrowIfCancellationRequested();

                                if (httpResponseString?.Length != contentLength)
                                {
                                    throw new JsonRpcRequestException(Strings.GetString("protocol.http.headers.invalid_set"), httpResponseMessage.StatusCode);
                                }

                                _jsonRpcSerializer.DynamicResponseBindings[request.Id] = contract;

                                var responseData = default(JsonRpcData<JsonRpcResponse>);

                                try
                                {
                                    responseData = _jsonRpcSerializer.DeserializeResponseData(httpResponseString);
                                }
                                catch (JsonRpcException e)
                                {
                                    throw new JsonRpcContractException(Strings.GetString("protocol.rpc.message.invalid_value"), e);
                                }
                                finally
                                {
                                    _jsonRpcSerializer.DynamicResponseBindings.Remove(request.Id);
                                }

                                cancellationToken.ThrowIfCancellationRequested();

                                if (responseData.IsBatch)
                                {
                                    throw new JsonRpcContractException(Strings.GetString("protocol.service.message.invalid_value"));
                                }

                                var jsonRpcItem = responseData.Item;

                                if (!jsonRpcItem.IsValid)
                                {
                                    throw new JsonRpcContractException(Strings.GetString("protocol.service.message.invalid_value"), jsonRpcItem.Exception);
                                }

                                var response = jsonRpcItem.Message;

                                if (!response.Success)
                                {
                                    throw new JsonRpcServiceException(response.Error.Code, response.Error.Message);
                                }

                                return response;
                            }
                        case HttpStatusCode.NoContent:
                            {
                                if (contract != null)
                                {
                                    throw new JsonRpcContractException(Strings.GetString("protocol.service.message.invalid_value"));
                                }

                                return null;
                            }
                        default:
                            {
                                throw new JsonRpcRequestException(Strings.GetString("protocol.http.status_code.invalid_value"), httpResponseMessage.StatusCode);
                            }
                    }
                }
            }
        }

        /// <summary>Releases all resources used by the current instance of the <see cref="JsonRpcClient" />.</summary>
        public void Dispose()
        {
            _httpMessageInvoker.Dispose();
            _jsonRpcSerializer.Dispose();
        }

        private static JsonRpcResponseContract CreateContract<T>()
        {
            return typeof(T) != typeof(VoidValue) ? JsonRpcResponseContract<T>.Instance : null;
        }

        private static JsonRpcId CreateIdentifier<T>()
        {
            return typeof(T) != typeof(VoidValue) ? new JsonRpcId(Guid.NewGuid().ToString("D")) : default;
        }

        private static HttpMessageInvoker CreateHttpMessageInvoker()
        {
            var httpHandler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            var httpClient = new HttpClient(httpHandler);

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_mediaTypeHeaderValue.MediaType));
            httpClient.DefaultRequestHeaders.ExpectContinue = false;

            return httpClient;
        }
    }
}