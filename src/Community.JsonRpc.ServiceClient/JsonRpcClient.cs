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
    public class JsonRpcClient : IDisposable
    {
        private static readonly MediaTypeHeaderValue _mediaTypeValue = new MediaTypeHeaderValue("application/json");
        private static readonly MediaTypeWithQualityHeaderValue _mediaTypeWithQualityValue = new MediaTypeWithQualityHeaderValue("application/json");

        private readonly HttpMessageInvoker _httpInvoker;
        private readonly Uri _serviceUri;

        private readonly JsonRpcSerializer _serializer =
            new JsonRpcSerializer(
                EmptyDictionary<string, JsonRpcRequestContract>.Instance,
                EmptyDictionary<string, JsonRpcResponseContract>.Instance,
                EmptyDictionary<JsonRpcId, string>.Instance,
                new Dictionary<JsonRpcId, JsonRpcResponseContract>(1));

        /// <summary>Initializes a new instance of the <see cref="JsonRpcClient" /> class.</summary>
        /// <param name="serviceUri">The service URI.</param>
        /// <param name="httpInvoker">The component for sending HTTP requests.</param>
        /// <exception cref="ArgumentNullException"><paramref name="serviceUri" /> is <see langword="null" />.</exception>
        /// <exception cref="FormatException"><paramref name="serviceUri" /> is a relative URI or is not correctly formed.</exception>
        public JsonRpcClient(string serviceUri, HttpMessageInvoker httpInvoker = null)
        {
            if (serviceUri == null)
            {
                throw new ArgumentNullException(nameof(serviceUri));
            }

            _serviceUri = new Uri(serviceUri, UriKind.Absolute);
            _httpInvoker = httpInvoker ?? CreateHttpInvoker();
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcClient" /> class.</summary>
        /// <param name="serviceUri">The service URI.</param>
        /// <param name="httpInvoker">The component for sending HTTP requests.</param>
        /// <exception cref="ArgumentException"><paramref name="serviceUri" /> is a relative URI.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="serviceUri" /> is <see langword="null" />.</exception>
        public JsonRpcClient(Uri serviceUri, HttpMessageInvoker httpInvoker = null)
        {
            if (serviceUri == null)
            {
                throw new ArgumentNullException(nameof(serviceUri));
            }
            if (!serviceUri.IsAbsoluteUri)
            {
                throw new ArgumentException(Strings.GetString("client.uri.relative"), nameof(serviceUri));
            }

            _serviceUri = serviceUri;
            _httpInvoker = httpInvoker ?? CreateHttpInvoker();
        }

        /// <summary>Invokes the specified service method.</summary>
        /// <typeparam name="T">The type of the service method result.</typeparam>
        /// <param name="method">The name of the service method.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the service method result.</returns>
        /// <exception cref="ArgumentException"><paramref name="method" /> is a system extension method.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcContractException">An error occurred during parameters or service result handling.</exception>
        /// <exception cref="JsonRpcRequestException">An error occurred during HTTP request execution.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
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

            cancellationToken.ThrowIfCancellationRequested();

            var request = new JsonRpcRequest(method, CreateIdentifier<T>());
            var response = await InvokeAsync(request, CreateContract<T>(), cancellationToken).ConfigureAwait(false);

            return response != null ? (T)response.Result : default;
        }

        /// <summary>Invokes the specified service method.</summary>
        /// <typeparam name="T">The type of the service method result.</typeparam>
        /// <param name="method">The name of the service method.</param>
        /// <param name="parameters">The parameters to be used during the invocation of the service method, specified by position.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the service method result.</returns>
        /// <exception cref="ArgumentException"><paramref name="method" /> is a system extension method or parameters count equals zero.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> or <paramref name="parameters" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcContractException">An error occurred during parameters or service result handling.</exception>
        /// <exception cref="JsonRpcRequestException">An error occurred during HTTP request execution.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
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

            cancellationToken.ThrowIfCancellationRequested();

            var request = new JsonRpcRequest(method, CreateIdentifier<T>(), parameters);
            var response = await InvokeAsync(request, CreateContract<T>(), cancellationToken).ConfigureAwait(false);

            return response != null ? (T)response.Result : default;
        }

        /// <summary>Invokes the specified service method.</summary>
        /// <typeparam name="T">The type of the service method result.</typeparam>
        /// <param name="method">The name of the service method.</param>
        /// <param name="parameters">The parameters to be used during the invocation of the service method, specified by name.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the service method result.</returns>
        /// <exception cref="ArgumentException"><paramref name="method" /> is a system extension method.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> or <paramref name="parameters" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcContractException">An error occurred during parameters or service result handling.</exception>
        /// <exception cref="JsonRpcRequestException">An error occurred during HTTP request execution.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
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

            cancellationToken.ThrowIfCancellationRequested();

            var request = new JsonRpcRequest(method, CreateIdentifier<T>(), parameters);
            var response = await InvokeAsync(request, CreateContract<T>(), cancellationToken).ConfigureAwait(false);

            return response != null ? (T)response.Result : default;
        }

        private async Task<JsonRpcResponse> InvokeAsync(JsonRpcRequest request, JsonRpcResponseContract contract, CancellationToken cancellationToken)
        {
            var requestString = default(string);

            try
            {
                requestString = _serializer.SerializeRequest(request);
            }
            catch (JsonRpcException e)
            {
                throw new JsonRpcContractException(request.Id.ToString(), Strings.GetString("invoke.params.invalid_values"), e);
            }

            cancellationToken.ThrowIfCancellationRequested();

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, _serviceUri))
            {
                var httpVersion = HttpVersion;

                if (httpVersion != null)
                {
                    requestMessage.Version = httpVersion;
                }

                VisitRequestHeaders(requestMessage.Headers);

                requestMessage.Headers.Accept.Clear();
                requestMessage.Headers.Accept.Add(_mediaTypeWithQualityValue);

                var requestContent = new StringContent(requestString);

                requestContent.Headers.ContentType = _mediaTypeValue;
                requestMessage.Content = requestContent;

                using (var responseMessage = await _httpInvoker.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false))
                {
                    VisitResponseHeaders(responseMessage.Headers);

                    switch (responseMessage.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            {
                                if (contract == null)
                                {
                                    throw new JsonRpcContractException(request.Id.ToString(), Strings.GetString("protocol.service.message.unexpected_content"));
                                }

                                var contentType = responseMessage.Content.Headers.ContentType;

                                if (contentType == null)
                                {
                                    throw new JsonRpcRequestException(responseMessage.StatusCode, Strings.GetString("protocol.http.headers.content_type.missing_value"));
                                }
                                if (string.Compare(contentType.MediaType, _mediaTypeValue.MediaType, StringComparison.OrdinalIgnoreCase) != 0)
                                {
                                    throw new JsonRpcRequestException(responseMessage.StatusCode, Strings.GetString("protocol.http.headers.content_type.invalid_value"));
                                }

                                var contentLength = responseMessage.Content.Headers.ContentLength;

                                if (contentLength == null)
                                {
                                    throw new JsonRpcRequestException(responseMessage.StatusCode, Strings.GetString("protocol.http.headers.content_length.missing_value"));
                                }

                                var responseString = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

                                cancellationToken.ThrowIfCancellationRequested();

                                if (responseString?.Length != contentLength)
                                {
                                    throw new JsonRpcRequestException(responseMessage.StatusCode, Strings.GetString("protocol.http.headers.content_length.invalid_value"));
                                }

                                _serializer.DynamicResponseBindings[request.Id] = contract;

                                var responseData = default(JsonRpcData<JsonRpcResponse>);

                                try
                                {
                                    responseData = _serializer.DeserializeResponseData(responseString);
                                }
                                catch (JsonRpcException e)
                                {
                                    throw new JsonRpcContractException(request.Id.ToString(), Strings.GetString("protocol.rpc.message.invalid_value"), e);
                                }
                                finally
                                {
                                    _serializer.DynamicResponseBindings.Remove(request.Id);
                                }

                                cancellationToken.ThrowIfCancellationRequested();

                                if (responseData.IsBatch)
                                {
                                    throw new JsonRpcContractException(request.Id.ToString(), Strings.GetString("protocol.service.message.batch_value"));
                                }

                                var responseItem = responseData.Item;

                                if (!responseItem.IsValid)
                                {
                                    throw new JsonRpcContractException(request.Id.ToString(), Strings.GetString("protocol.service.message.invalid_value"), responseItem.Exception);
                                }

                                var response = responseItem.Message;

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
                                    throw new JsonRpcContractException(request.Id.ToString(), Strings.GetString("protocol.service.message.unexpected_blank"));
                                }

                                return null;
                            }
                        default:
                            {
                                throw new JsonRpcRequestException(responseMessage.StatusCode, Strings.GetString("protocol.http.status_code.invalid_value"));
                            }
                    }
                }
            }
        }

        /// <summary>Releases all resources used by the current instance of the <see cref="JsonRpcClient" />.</summary>
        public void Dispose()
        {
            _httpInvoker.Dispose();
            _serializer.Dispose();
        }

        /// <summary>Visits request headers.</summary>
        /// <param name="headers">A collection of request headers.</param>
        protected virtual void VisitRequestHeaders(HttpRequestHeaders headers)
        {
        }

        /// <summary>Visits response headers.</summary>
        /// <param name="headers">A collection of response headers.</param>
        protected virtual void VisitResponseHeaders(HttpResponseHeaders headers)
        {
        }

        private static JsonRpcResponseContract CreateContract<T>()
        {
            return typeof(T) != typeof(VoidValue) ? JsonRpcResponseContract<T>.Instance : null;
        }

        private static JsonRpcId CreateIdentifier<T>()
        {
            return typeof(T) != typeof(VoidValue) ? new JsonRpcId(Guid.NewGuid().ToString("D")) : default;
        }

        private static HttpMessageInvoker CreateHttpInvoker()
        {
            var httpHandler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            var httpClient = new HttpClient(httpHandler);

            httpClient.DefaultRequestHeaders.ExpectContinue = false;

            return httpClient;
        }

        /// <summary>Gets the HTTP message version.</summary>
        protected virtual Version HttpVersion
        {
            get => null;
        }
    }
}