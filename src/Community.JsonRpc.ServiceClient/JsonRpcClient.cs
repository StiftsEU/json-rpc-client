// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Data.JsonRpc;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Community.JsonRpc.ServiceClient.Resources;
using Newtonsoft.Json;

namespace Community.JsonRpc.ServiceClient
{
    /// <summary>Represents a JSON-RPC 2.0 service client.</summary>
    public class JsonRpcClient : IDisposable
    {
        private static readonly MediaTypeHeaderValue _mediaTypeValue = new MediaTypeHeaderValue("application/json");
        private static readonly MediaTypeWithQualityHeaderValue _mediaTypeWithQualityValue = new MediaTypeWithQualityHeaderValue("application/json");

        private readonly Uri _serviceUri;
        private readonly HttpMessageInvoker _httpInvoker;
        private readonly JsonRpcContractResolver _jsonRpcContractResolver;
        private readonly JsonRpcSerializer _jsonRpcSerializer;

        /// <summary>Initializes a new instance of the <see cref="JsonRpcClient" /> class.</summary>
        /// <param name="serviceUri">The service URI.</param>
        /// <param name="httpInvoker">The component for sending HTTP requests.</param>
        /// <exception cref="ArgumentNullException"><paramref name="serviceUri" /> or <paramref name="httpInvoker" /> is <see langword="null" />.</exception>
        /// <exception cref="FormatException"><paramref name="serviceUri" /> is a relative URI or is not correctly formed.</exception>
        public JsonRpcClient(string serviceUri, HttpMessageInvoker httpInvoker)
        {
            if (serviceUri == null)
            {
                throw new ArgumentNullException(nameof(serviceUri));
            }
            if (httpInvoker == null)
            {
                throw new ArgumentNullException(nameof(httpInvoker));
            }

            _serviceUri = new Uri(serviceUri, UriKind.Absolute);
            _httpInvoker = httpInvoker;
            _jsonRpcContractResolver = new JsonRpcContractResolver();
            _jsonRpcSerializer = new JsonRpcSerializer(_jsonRpcContractResolver);
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcClient" /> class.</summary>
        /// <param name="serviceUri">The service URI.</param>
        /// <exception cref="ArgumentNullException"><paramref name="serviceUri" /> is <see langword="null" />.</exception>
        /// <exception cref="FormatException"><paramref name="serviceUri" /> is a relative URI or is not correctly formed.</exception>
        public JsonRpcClient(string serviceUri)
            : this(serviceUri, CreateHttpInvoker())
        {
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcClient" /> class.</summary>
        /// <param name="serviceUri">The service URI.</param>
        /// <param name="httpInvoker">The component for sending HTTP requests.</param>
        /// <exception cref="ArgumentException"><paramref name="serviceUri" /> is a relative URI.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="serviceUri" /> or <paramref name="httpInvoker" /> is <see langword="null" />.</exception>
        public JsonRpcClient(Uri serviceUri, HttpMessageInvoker httpInvoker)
        {
            if (serviceUri == null)
            {
                throw new ArgumentNullException(nameof(serviceUri));
            }
            if (!serviceUri.IsAbsoluteUri)
            {
                throw new ArgumentException(Strings.GetString("client.uri.relative"), nameof(serviceUri));
            }
            if (httpInvoker == null)
            {
                throw new ArgumentNullException(nameof(httpInvoker));
            }

            _serviceUri = serviceUri;
            _httpInvoker = httpInvoker;
            _jsonRpcContractResolver = new JsonRpcContractResolver();
            _jsonRpcSerializer = new JsonRpcSerializer(_jsonRpcContractResolver);
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcClient" /> class.</summary>
        /// <param name="serviceUri">The service URI.</param>
        /// <exception cref="ArgumentException"><paramref name="serviceUri" /> is a relative URI.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="serviceUri" /> is <see langword="null" />.</exception>
        public JsonRpcClient(Uri serviceUri)
            : this(serviceUri, CreateHttpInvoker())
        {
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

        private Task<JsonRpcResponse> InvokeAsync(JsonRpcRequest request, JsonRpcResponseContract contract, CancellationToken cancellationToken)
        {
            _jsonRpcContractResolver.AddResponseContract(request.Id, contract);

            try
            {
                return SendJsonRpcRequestAsync(request, cancellationToken);
            }
            finally
            {
                _jsonRpcContractResolver.RemoveResponseContract(request.Id);
            }
        }

        /// <summary>Sends the specified JSON-RPC request as an asynchronous operation.</summary>
        /// <param name="request">The JSON-RPC request to send.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the JSON-RPC response.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="request" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcContractException">An error occurred during parameters or JSON-RPC response handling.</exception>
        /// <exception cref="JsonRpcRequestException">An error occurred during HTTP request execution.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during JSON-RPC method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        protected async Task<JsonRpcResponse> SendJsonRpcRequestAsync(JsonRpcRequest request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            using (var requestStream = new MemoryStream())
            {
                try
                {
                    _jsonRpcSerializer.SerializeRequest(request, requestStream);
                }
                catch (JsonException e)
                {
                    throw new JsonRpcContractException(Strings.GetString("invoke.params.invalid_values"), e, request.Id.ToString());
                }
                catch (JsonRpcException e)
                {
                    throw new JsonRpcContractException(Strings.GetString("invoke.params.invalid_values"), e, e.MessageId.ToString());
                }

                cancellationToken.ThrowIfCancellationRequested();
                requestStream.Position = 0;

                using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, _serviceUri))
                {
                    var httpProtocolVersion = HttpProtocolVersion;

                    if (httpProtocolVersion != null)
                    {
                        httpRequest.Version = httpProtocolVersion;
                    }

                    VisitHttpRequestHeaders(httpRequest.Headers);

                    httpRequest.Headers.Accept.Clear();
                    httpRequest.Headers.Accept.Add(_mediaTypeWithQualityValue);

                    var requestContent = new StreamContent(requestStream);

                    requestContent.Headers.ContentType = _mediaTypeValue;
                    httpRequest.Content = requestContent;

                    using (var httpResponse = await _httpInvoker.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false))
                    {
                        VisitHttpResponseHeaders(httpResponse.Headers);

                        switch (httpResponse.StatusCode)
                        {
                            case HttpStatusCode.OK:
                                {
                                    if (request.Id.Type == JsonRpcIdType.None)
                                    {
                                        throw new JsonRpcContractException(Strings.GetString("protocol.service.message.unexpected_content"), request.Id.ToString());
                                    }

                                    var contentType = httpResponse.Content.Headers.ContentType;

                                    if (contentType == null)
                                    {
                                        throw new JsonRpcRequestException(httpResponse.StatusCode, Strings.GetString("protocol.http.headers.content_type.missing_value"));
                                    }
                                    if (string.Compare(contentType.MediaType, _mediaTypeValue.MediaType, StringComparison.OrdinalIgnoreCase) != 0)
                                    {
                                        throw new JsonRpcRequestException(httpResponse.StatusCode, Strings.GetString("protocol.http.headers.content_type.invalid_value"));
                                    }

                                    var responseData = default(JsonRpcData<JsonRpcResponse>);

                                    using (var responseStream = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false))
                                    {
                                        cancellationToken.ThrowIfCancellationRequested();

                                        try
                                        {
                                            responseData = await _jsonRpcSerializer.DeserializeResponseDataAsync(responseStream, cancellationToken).ConfigureAwait(false);
                                        }
                                        catch (JsonException e)
                                        {
                                            throw new JsonRpcContractException(Strings.GetString("protocol.rpc.message.invalid_value"), e, request.Id.ToString());
                                        }
                                        catch (JsonRpcException e)
                                        {
                                            throw new JsonRpcContractException(Strings.GetString("protocol.rpc.message.invalid_value"), e, e.MessageId.ToString());
                                        }
                                    }

                                    if (responseData.IsBatch)
                                    {
                                        throw new JsonRpcContractException(Strings.GetString("protocol.service.message.batch_value"), request.Id.ToString());
                                    }

                                    var responseItem = responseData.Item;

                                    if (!responseItem.IsValid)
                                    {
                                        throw new JsonRpcContractException(Strings.GetString("protocol.service.message.invalid_value"), responseItem.Exception, request.Id.ToString());
                                    }

                                    var response = responseItem.Message;

                                    if (!response.Success)
                                    {
                                        throw new JsonRpcServiceException(response.Error.Code, response.Error.Message, response.Error.Data, response.Error.HasData);
                                    }

                                    return response;
                                }
                            case HttpStatusCode.NoContent:
                                {
                                    if (request.Id.Type != JsonRpcIdType.None)
                                    {
                                        throw new JsonRpcContractException(request.Id.ToString(), Strings.GetString("protocol.service.message.unexpected_blank"));
                                    }

                                    return null;
                                }
                            default:
                                {
                                    throw new JsonRpcRequestException(httpResponse.StatusCode, Strings.GetString("protocol.http.status_code.invalid_value"));
                                }
                        }
                    }
                }
            }
        }

        /// <summary>Sends the specified JSON-RPC request as an asynchronous operation.</summary>
        /// <param name="requests">The JSON-RPC requests to send.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the JSON-RPC responses.</returns>
        /// <exception cref="AggregateException">An error occurred during JSON-RPC response handling.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="requests" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcContractException">An error occurred during parameters or JSON-RPC response handling.</exception>
        /// <exception cref="JsonRpcRequestException">An error occurred during HTTP request execution.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during JSON-RPC method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        protected async Task<IReadOnlyList<JsonRpcResponse>> SendJsonRpcRequestsAsync(IReadOnlyList<JsonRpcRequest> requests, CancellationToken cancellationToken)
        {
            if (requests == null)
            {
                throw new ArgumentNullException(nameof(requests));
            }

            var requestIdentifiers = new HashSet<JsonRpcId>();

            for (var i = 0; i < requests.Count; i++)
            {
                var identifier = requests[i].Id;

                if (identifier.Type == JsonRpcIdType.None)
                {
                    continue;
                }
                if (!requestIdentifiers.Add(identifier))
                {
                    throw new JsonRpcContractException(Strings.GetString("protocol.service.message.duplicate_identifiers"));
                }
            }

            using (var requestStream = new MemoryStream())
            {
                try
                {
                    _jsonRpcSerializer.SerializeRequests(requests, requestStream);
                }
                catch (JsonException e)
                {
                    throw new JsonRpcContractException(Strings.GetString("invoke.params.invalid_values"), e);
                }
                catch (JsonRpcException e)
                {
                    throw new JsonRpcContractException(Strings.GetString("invoke.params.invalid_values"), e, e.MessageId.ToString());
                }

                cancellationToken.ThrowIfCancellationRequested();
                requestStream.Position = 0;

                using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, _serviceUri))
                {
                    var httpProtocolVersion = HttpProtocolVersion;

                    if (httpProtocolVersion != null)
                    {
                        httpRequest.Version = httpProtocolVersion;
                    }

                    VisitHttpRequestHeaders(httpRequest.Headers);

                    httpRequest.Headers.Accept.Clear();
                    httpRequest.Headers.Accept.Add(_mediaTypeWithQualityValue);

                    var requestContent = new StreamContent(requestStream);

                    requestContent.Headers.ContentType = _mediaTypeValue;
                    httpRequest.Content = requestContent;

                    using (var httpResponse = await _httpInvoker.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false))
                    {
                        VisitHttpResponseHeaders(httpResponse.Headers);

                        switch (httpResponse.StatusCode)
                        {
                            case HttpStatusCode.OK:
                                {
                                    var contentType = httpResponse.Content.Headers.ContentType;

                                    if (contentType == null)
                                    {
                                        throw new JsonRpcRequestException(httpResponse.StatusCode, Strings.GetString("protocol.http.headers.content_type.missing_value"));
                                    }
                                    if (string.Compare(contentType.MediaType, _mediaTypeValue.MediaType, StringComparison.OrdinalIgnoreCase) != 0)
                                    {
                                        throw new JsonRpcRequestException(httpResponse.StatusCode, Strings.GetString("protocol.http.headers.content_type.invalid_value"));
                                    }

                                    var responseData = default(JsonRpcData<JsonRpcResponse>);

                                    using (var responseStream = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false))
                                    {
                                        cancellationToken.ThrowIfCancellationRequested();

                                        try
                                        {
                                            responseData = await _jsonRpcSerializer.DeserializeResponseDataAsync(responseStream, cancellationToken).ConfigureAwait(false);
                                        }
                                        catch (JsonException e)
                                        {
                                            throw new JsonRpcContractException(Strings.GetString("protocol.rpc.message.invalid_value"), e, ToString());
                                        }
                                        catch (JsonRpcException e)
                                        {
                                            throw new JsonRpcContractException(Strings.GetString("protocol.rpc.message.invalid_value"), e, e.MessageId.ToString());
                                        }
                                    }

                                    if (responseData.IsBatch)
                                    {
                                        var responseItems = responseData.Items;
                                        var responseIdentifiers = new HashSet<JsonRpcId>();
                                        var responses = new JsonRpcResponse[responseItems.Count];
                                        var exceptions = new HashSet<JsonRpcContractException>();

                                        for (var i = 0; i < responseItems.Count; i++)
                                        {
                                            var responseItem = responseItems[i];

                                            if (responseItem.IsValid)
                                            {
                                                var response = responseItem.Message;

                                                responses[i] = response;
                                                responseIdentifiers.Add(response.Id);
                                            }
                                            else
                                            {
                                                exceptions.Add(new JsonRpcContractException(Strings.GetString("protocol.service.message.invalid_value"), responseItem.Exception));
                                            }
                                        }

                                        if (exceptions.Count != 0)
                                        {
                                            throw new AggregateException(Strings.GetString("protocol.service.message.invalid_value"), exceptions);
                                        }
                                        if (!requestIdentifiers.SetEquals(responseIdentifiers))
                                        {
                                            throw new JsonRpcContractException(Strings.GetString("protocol.service.message.invalid_values"));
                                        }

                                        return responses;
                                    }
                                    else
                                    {
                                        var responseItem = responseData.Item;

                                        if (!responseItem.IsValid)
                                        {
                                            throw new JsonRpcContractException(Strings.GetString("protocol.service.message.invalid_value"), responseItem.Exception);
                                        }

                                        var response = responseItem.Message;

                                        if (!response.Success)
                                        {
                                            throw new JsonRpcServiceException(response.Error.Code, response.Error.Message, response.Error.Data, response.Error.HasData);
                                        }
                                        else
                                        {
                                            throw new JsonRpcContractException(Strings.GetString("protocol.service.message.single_value"));
                                        }
                                    }
                                }
                            case HttpStatusCode.NoContent:
                                {
                                    if (requestIdentifiers.Count != 0)
                                    {
                                        throw new JsonRpcContractException(Strings.GetString("protocol.service.message.invalid_values"));
                                    }

                                    return new JsonRpcResponse[] { };
                                }
                            default:
                                {
                                    throw new JsonRpcRequestException(httpResponse.StatusCode, Strings.GetString("protocol.http.status_code.invalid_value"));
                                }
                        }
                    }
                }
            }
        }

        /// <summary>Visits HTTP request headers.</summary>
        /// <param name="headers">A collection of request headers.</param>
        protected virtual void VisitHttpRequestHeaders(HttpRequestHeaders headers)
        {
        }

        /// <summary>Visits HTTP response headers.</summary>
        /// <param name="headers">A collection of response headers.</param>
        protected virtual void VisitHttpResponseHeaders(HttpResponseHeaders headers)
        {
        }

        /// <summary>Releases all resources used by the current instance of the <see cref="JsonRpcClient" />.</summary>
        public void Dispose()
        {
            _httpInvoker.Dispose();
        }

        /// <summary>Invokes the specified service method as an asynchronous operation.</summary>
        /// <param name="method">The name of the service method.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentException"><paramref name="method" /> is a system extension method.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcContractException">An error occurred during parameters or service result handling.</exception>
        /// <exception cref="JsonRpcRequestException">An error occurred during HTTP request execution.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public Task InvokeAsync(string method, CancellationToken cancellationToken = default)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (JsonRpcSerializer.IsSystemMethod(method))
            {
                throw new ArgumentException(Strings.GetString("invoke.method.invalid_name"), nameof(method));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var request = new JsonRpcRequest(method);

            return SendJsonRpcRequestAsync(request, cancellationToken);
        }

        /// <summary>Invokes the specified service method as an asynchronous operation.</summary>
        /// <param name="method">The name of the service method.</param>
        /// <param name="parameters">The parameters to be used during the invocation of the service method, specified by position.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentException"><paramref name="method" /> is a system extension method.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> or <paramref name="parameters" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcContractException">An error occurred during parameters or service result handling.</exception>
        /// <exception cref="JsonRpcRequestException">An error occurred during HTTP request execution.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public Task InvokeAsync(string method, IReadOnlyList<object> parameters, CancellationToken cancellationToken = default)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (JsonRpcSerializer.IsSystemMethod(method))
            {
                throw new ArgumentException(Strings.GetString("invoke.method.invalid_name"), nameof(method));
            }
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var request = new JsonRpcRequest(method, parameters);

            return SendJsonRpcRequestAsync(request, cancellationToken);
        }

        /// <summary>Invokes the specified service method as an asynchronous operation.</summary>
        /// <param name="method">The name of the service method.</param>
        /// <param name="parameters">The parameters to be used during the invocation of the service method, specified by name.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentException"><paramref name="method" /> is a system extension method.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> or <paramref name="parameters" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcContractException">An error occurred during parameters or service result handling.</exception>
        /// <exception cref="JsonRpcRequestException">An error occurred during HTTP request execution.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public Task InvokeAsync(string method, IReadOnlyDictionary<string, object> parameters, CancellationToken cancellationToken = default)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (JsonRpcSerializer.IsSystemMethod(method))
            {
                throw new ArgumentException(Strings.GetString("invoke.method.invalid_name"), nameof(method));
            }
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var request = new JsonRpcRequest(method, parameters);

            return SendJsonRpcRequestAsync(request, cancellationToken);
        }

        /// <summary>Invokes the specified service method as an asynchronous operation.</summary>
        /// <typeparam name="TResult">The type of the service method result.</typeparam>
        /// <param name="method">The name of the service method.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the service method result.</returns>
        /// <exception cref="ArgumentException"><paramref name="method" /> is a system extension method.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcContractException">An error occurred during parameters or service result handling.</exception>
        /// <exception cref="JsonRpcRequestException">An error occurred during HTTP request execution.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<TResult> InvokeAsync<TResult>(string method, CancellationToken cancellationToken = default)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (JsonRpcSerializer.IsSystemMethod(method))
            {
                throw new ArgumentException(Strings.GetString("invoke.method.invalid_name"), nameof(method));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var request = new JsonRpcRequest(method, Guid.NewGuid().ToString("D"));
            var response = await InvokeAsync(request, JsonRpcResponseContract<TResult>.Instance, cancellationToken).ConfigureAwait(false);

            return (TResult)response.Result;
        }

        /// <summary>Invokes the specified service method as an asynchronous operation.</summary>
        /// <typeparam name="TResult">The type of the service method result.</typeparam>
        /// <param name="method">The name of the service method.</param>
        /// <param name="parameters">The parameters to be used during the invocation of the service method, specified by position.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the service method result.</returns>
        /// <exception cref="ArgumentException"><paramref name="method" /> is a system extension method.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> or <paramref name="parameters" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcContractException">An error occurred during parameters or service result handling.</exception>
        /// <exception cref="JsonRpcRequestException">An error occurred during HTTP request execution.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<TResult> InvokeAsync<TResult>(string method, IReadOnlyList<object> parameters, CancellationToken cancellationToken = default)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (JsonRpcSerializer.IsSystemMethod(method))
            {
                throw new ArgumentException(Strings.GetString("invoke.method.invalid_name"), nameof(method));
            }
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var request = new JsonRpcRequest(method, Guid.NewGuid().ToString("D"), parameters);
            var response = await InvokeAsync(request, JsonRpcResponseContract<TResult>.Instance, cancellationToken).ConfigureAwait(false);

            return (TResult)response.Result;
        }

        /// <summary>Invokes the specified service method as an asynchronous operation.</summary>
        /// <typeparam name="TResult">The type of the service method result.</typeparam>
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
        public async Task<TResult> InvokeAsync<TResult>(string method, IReadOnlyDictionary<string, object> parameters, CancellationToken cancellationToken = default)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (JsonRpcSerializer.IsSystemMethod(method))
            {
                throw new ArgumentException(Strings.GetString("invoke.method.invalid_name"), nameof(method));
            }
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var request = new JsonRpcRequest(method, Guid.NewGuid().ToString("D"), parameters);
            var response = await InvokeAsync(request, JsonRpcResponseContract<TResult>.Instance, cancellationToken).ConfigureAwait(false);

            return (TResult)response.Result;
        }

        /// <summary>Invokes the specified service method as an asynchronous operation.</summary>
        /// <typeparam name="TResult">The type of the service method result.</typeparam>
        /// <param name="method">The name of the service method.</param>
        /// <param name="identifier">The message identifier.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the service method result.</returns>
        /// <exception cref="ArgumentException"><paramref name="method" /> is a system extension method or <paramref name="identifier" /> is void.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcContractException">An error occurred during parameters or service result handling.</exception>
        /// <exception cref="JsonRpcRequestException">An error occurred during HTTP request execution.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<TResult> InvokeAsync<TResult>(string method, JsonRpcId identifier, CancellationToken cancellationToken = default)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (JsonRpcSerializer.IsSystemMethod(method))
            {
                throw new ArgumentException(Strings.GetString("invoke.method.invalid_name"), nameof(method));
            }
            if (identifier.Type == JsonRpcIdType.None)
            {
                throw new ArgumentException(Strings.GetString("invoke.identifier.invalid_value"), nameof(identifier));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var request = new JsonRpcRequest(method, identifier);
            var response = await InvokeAsync(request, JsonRpcResponseContract<TResult>.Instance, cancellationToken).ConfigureAwait(false);

            return (TResult)response.Result;
        }

        /// <summary>Invokes the specified service method as an asynchronous operation.</summary>
        /// <typeparam name="TResult">The type of the service method result.</typeparam>
        /// <param name="method">The name of the service method.</param>
        /// <param name="identifier">The message identifier.</param>
        /// <param name="parameters">The parameters to be used during the invocation of the service method, specified by position.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the service method result.</returns>
        /// <exception cref="ArgumentException"><paramref name="method" /> is a system extension method or <paramref name="identifier" /> is void.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> or <paramref name="parameters" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcContractException">An error occurred during parameters or service result handling.</exception>
        /// <exception cref="JsonRpcRequestException">An error occurred during HTTP request execution.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<TResult> InvokeAsync<TResult>(string method, JsonRpcId identifier, IReadOnlyList<object> parameters, CancellationToken cancellationToken = default)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (JsonRpcSerializer.IsSystemMethod(method))
            {
                throw new ArgumentException(Strings.GetString("invoke.method.invalid_name"), nameof(method));
            }
            if (identifier.Type == JsonRpcIdType.None)
            {
                throw new ArgumentException(Strings.GetString("invoke.identifier.invalid_value"), nameof(identifier));
            }
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var request = new JsonRpcRequest(method, identifier, parameters);
            var response = await InvokeAsync(request, JsonRpcResponseContract<TResult>.Instance, cancellationToken).ConfigureAwait(false);

            return (TResult)response.Result;
        }

        /// <summary>Invokes the specified service method as an asynchronous operation.</summary>
        /// <typeparam name="TResult">The type of the service method result.</typeparam>
        /// <param name="method">The name of the service method.</param>
        /// <param name="identifier">The message identifier.</param>
        /// <param name="parameters">The parameters to be used during the invocation of the service method, specified by name.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the service method result.</returns>
        /// <exception cref="ArgumentException"><paramref name="method" /> is a system extension method or <paramref name="identifier" /> is void.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> or <paramref name="parameters" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcContractException">An error occurred during parameters or service result handling.</exception>
        /// <exception cref="JsonRpcRequestException">An error occurred during HTTP request execution.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<TResult> InvokeAsync<TResult>(string method, JsonRpcId identifier, IReadOnlyDictionary<string, object> parameters, CancellationToken cancellationToken = default)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (JsonRpcSerializer.IsSystemMethod(method))
            {
                throw new ArgumentException(Strings.GetString("invoke.method.invalid_name"), nameof(method));
            }
            if (identifier.Type == JsonRpcIdType.None)
            {
                throw new ArgumentException(Strings.GetString("invoke.identifier.invalid_value"), nameof(identifier));
            }
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var request = new JsonRpcRequest(method, identifier, parameters);
            var response = await InvokeAsync(request, JsonRpcResponseContract<TResult>.Instance, cancellationToken).ConfigureAwait(false);

            return (TResult)response.Result;
        }

        /// <summary>Invokes the specified service method as an asynchronous operation.</summary>
        /// <typeparam name="TResult">The type of the service method result.</typeparam>
        /// <typeparam name="TErrorData">The type of the service method optional error data.</typeparam>
        /// <param name="method">The name of the service method.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the service method result.</returns>
        /// <exception cref="ArgumentException"><paramref name="method" /> is a system extension method.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcContractException">An error occurred during parameters or service result handling.</exception>
        /// <exception cref="JsonRpcRequestException">An error occurred during HTTP request execution.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<TResult> InvokeAsync<TResult, TErrorData>(string method, CancellationToken cancellationToken = default)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (JsonRpcSerializer.IsSystemMethod(method))
            {
                throw new ArgumentException(Strings.GetString("invoke.method.invalid_name"), nameof(method));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var request = new JsonRpcRequest(method, Guid.NewGuid().ToString("D"));
            var response = await InvokeAsync(request, JsonRpcResponseContract<TResult, TErrorData>.Instance, cancellationToken).ConfigureAwait(false);

            return (TResult)response.Result;
        }

        /// <summary>Invokes the specified service method as an asynchronous operation.</summary>
        /// <typeparam name="TResult">The type of the service method result.</typeparam>
        /// <typeparam name="TErrorData">The type of the service method optional error data.</typeparam>
        /// <param name="method">The name of the service method.</param>
        /// <param name="parameters">The parameters to be used during the invocation of the service method, specified by position.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the service method result.</returns>
        /// <exception cref="ArgumentException"><paramref name="method" /> is a system extension method.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> or <paramref name="parameters" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcContractException">An error occurred during parameters or service result handling.</exception>
        /// <exception cref="JsonRpcRequestException">An error occurred during HTTP request execution.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<TResult> InvokeAsync<TResult, TErrorData>(string method, IReadOnlyList<object> parameters, CancellationToken cancellationToken = default)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (JsonRpcSerializer.IsSystemMethod(method))
            {
                throw new ArgumentException(Strings.GetString("invoke.method.invalid_name"), nameof(method));
            }
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var request = new JsonRpcRequest(method, Guid.NewGuid().ToString("D"), parameters);
            var response = await InvokeAsync(request, JsonRpcResponseContract<TResult>.Instance, cancellationToken).ConfigureAwait(false);

            return (TResult)response.Result;
        }

        /// <summary>Invokes the specified service method as an asynchronous operation.</summary>
        /// <typeparam name="TResult">The type of the service method result.</typeparam>
        /// <typeparam name="TErrorData">The type of the service method optional error data.</typeparam>
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
        public async Task<TResult> InvokeAsync<TResult, TErrorData>(string method, IReadOnlyDictionary<string, object> parameters, CancellationToken cancellationToken = default)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (JsonRpcSerializer.IsSystemMethod(method))
            {
                throw new ArgumentException(Strings.GetString("invoke.method.invalid_name"), nameof(method));
            }
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var request = new JsonRpcRequest(method, Guid.NewGuid().ToString("D"), parameters);
            var response = await InvokeAsync(request, JsonRpcResponseContract<TResult, TErrorData>.Instance, cancellationToken).ConfigureAwait(false);

            return (TResult)response.Result;
        }

        /// <summary>Invokes the specified service method as an asynchronous operation.</summary>
        /// <typeparam name="TResult">The type of the service method result.</typeparam>
        /// <typeparam name="TErrorData">The type of the service method optional error data.</typeparam>
        /// <param name="method">The name of the service method.</param>
        /// <param name="identifier">The message identifier.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the service method result.</returns>
        /// <exception cref="ArgumentException"><paramref name="method" /> is a system extension method or <paramref name="identifier" /> is void.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcContractException">An error occurred during parameters or service result handling.</exception>
        /// <exception cref="JsonRpcRequestException">An error occurred during HTTP request execution.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<TResult> InvokeAsync<TResult, TErrorData>(string method, JsonRpcId identifier, CancellationToken cancellationToken = default)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (JsonRpcSerializer.IsSystemMethod(method))
            {
                throw new ArgumentException(Strings.GetString("invoke.method.invalid_name"), nameof(method));
            }
            if (identifier.Type == JsonRpcIdType.None)
            {
                throw new ArgumentException(Strings.GetString("invoke.identifier.invalid_value"), nameof(identifier));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var request = new JsonRpcRequest(method, identifier);
            var response = await InvokeAsync(request, JsonRpcResponseContract<TResult, TErrorData>.Instance, cancellationToken).ConfigureAwait(false);

            return (TResult)response.Result;
        }

        /// <summary>Invokes the specified service method as an asynchronous operation.</summary>
        /// <typeparam name="TResult">The type of the service method result.</typeparam>
        /// <typeparam name="TErrorData">The type of the service method optional error data.</typeparam>
        /// <param name="method">The name of the service method.</param>
        /// <param name="identifier">The message identifier.</param>
        /// <param name="parameters">The parameters to be used during the invocation of the service method, specified by position.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the service method result.</returns>
        /// <exception cref="ArgumentException"><paramref name="method" /> is a system extension method or <paramref name="identifier" /> is void.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> or <paramref name="parameters" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcContractException">An error occurred during parameters or service result handling.</exception>
        /// <exception cref="JsonRpcRequestException">An error occurred during HTTP request execution.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<TResult> InvokeAsync<TResult, TErrorData>(string method, JsonRpcId identifier, IReadOnlyList<object> parameters, CancellationToken cancellationToken = default)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (JsonRpcSerializer.IsSystemMethod(method))
            {
                throw new ArgumentException(Strings.GetString("invoke.method.invalid_name"), nameof(method));
            }
            if (identifier.Type == JsonRpcIdType.None)
            {
                throw new ArgumentException(Strings.GetString("invoke.identifier.invalid_value"), nameof(identifier));
            }
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var request = new JsonRpcRequest(method, identifier, parameters);
            var response = await InvokeAsync(request, JsonRpcResponseContract<TResult, TErrorData>.Instance, cancellationToken).ConfigureAwait(false);

            return (TResult)response.Result;
        }

        /// <summary>Invokes the specified service method as an asynchronous operation.</summary>
        /// <typeparam name="TResult">The type of the service method result.</typeparam>
        /// <typeparam name="TErrorData">The type of the service method optional error data.</typeparam>
        /// <param name="method">The name of the service method.</param>
        /// <param name="identifier">The message identifier.</param>
        /// <param name="parameters">The parameters to be used during the invocation of the service method, specified by name.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the service method result.</returns>
        /// <exception cref="ArgumentException"><paramref name="method" /> is a system extension method or <paramref name="identifier" /> is void.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> or <paramref name="parameters" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcContractException">An error occurred during parameters or service result handling.</exception>
        /// <exception cref="JsonRpcRequestException">An error occurred during HTTP request execution.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during service method invocation.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<TResult> InvokeAsync<TResult, TErrorData>(string method, JsonRpcId identifier, IReadOnlyDictionary<string, object> parameters, CancellationToken cancellationToken = default)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (JsonRpcSerializer.IsSystemMethod(method))
            {
                throw new ArgumentException(Strings.GetString("invoke.method.invalid_name"), nameof(method));
            }
            if (identifier.Type == JsonRpcIdType.None)
            {
                throw new ArgumentException(Strings.GetString("invoke.identifier.invalid_value"), nameof(identifier));
            }
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var request = new JsonRpcRequest(method, identifier, parameters);
            var response = await InvokeAsync(request, JsonRpcResponseContract<TResult, TErrorData>.Instance, cancellationToken).ConfigureAwait(false);

            return (TResult)response.Result;
        }

        /// <summary>Gets the current JSON-RPC message contract resolver.</summary>
        protected JsonRpcContractResolver ContractResolver
        {
            get => _jsonRpcContractResolver;
        }

        /// <summary>Gets the current HTTP protocol version.</summary>
        protected virtual Version HttpProtocolVersion
        {
            get => null;
        }

        /// <summary>Gets the current service URI.</summary>
        protected Uri ServiceUri
        {
            get => _serviceUri;
        }
    }
}