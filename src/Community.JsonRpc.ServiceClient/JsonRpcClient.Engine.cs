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

#if NETCOREAPP2_1

using System.IO.Compression;

#endif

namespace Community.JsonRpc.ServiceClient
{
    public partial class JsonRpcClient
    {
        private const int _messageBufferSize = 64;

        private static readonly MediaTypeHeaderValue _mediaTypeValue = new MediaTypeHeaderValue("application/json");
        private static readonly MediaTypeWithQualityHeaderValue _mediaTypeWithQualityValue = new MediaTypeWithQualityHeaderValue("application/json");

#if NETCOREAPP2_1

        private static readonly StringWithQualityHeaderValue _brotliEncodingHeader = new StringWithQualityHeaderValue("br");

#endif

        private readonly JsonRpcContractResolver _jsonRpcContractResolver = new JsonRpcContractResolver();
        private readonly JsonRpcSerializer _jsonRpcSerializer;
        private readonly Uri _serviceUri;
        private readonly HttpMessageInvoker _httpInvoker;

        private static JsonSerializer CreateJsonSerializer()
        {
            var settings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            };

            return JsonSerializer.CreateDefault(settings);
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

#if NETCOREAPP2_1

        private static bool CheckHttpContentEncoding(HttpResponseMessage httpResponse, string encoding)
        {
            var contentEncodings = httpResponse.Content.Headers.ContentEncoding;

            if (contentEncodings.Count == 0)
            {
                return false;
            }

            var outerContentEncoding = default(string);

            foreach (var contentEncoding in contentEncodings)
            {
                outerContentEncoding = contentEncoding;
            }

            return string.Compare(outerContentEncoding, encoding, StringComparison.OrdinalIgnoreCase) == 0;
        }

#endif

        private void PrepareHttpRequest(HttpRequestMessage httpRequest)
        {
            VisitHttpRequestHeaders(httpRequest.Headers);

#if NETCOREAPP2_1

            if (!httpRequest.Headers.AcceptEncoding.Contains(_brotliEncodingHeader))
            {
                httpRequest.Headers.AcceptEncoding.Add(_brotliEncodingHeader);
            }

#endif

            httpRequest.Headers.Accept.Clear();
            httpRequest.Headers.Accept.Add(_mediaTypeWithQualityValue);
        }

        private Task<JsonRpcResponse> SendJsonRpcRequestAsync(JsonRpcRequest request, JsonRpcResponseContract contract, CancellationToken cancellationToken)
        {
            ref readonly var requestId = ref request.Id;

            _jsonRpcContractResolver.AddResponseContract(requestId, contract);

            try
            {
                return SendJsonRpcRequestAsync(request, cancellationToken);
            }
            finally
            {
                _jsonRpcContractResolver.RemoveResponseContract(requestId);
            }
        }

        /// <summary>Creates a unique JSON-RPC request identifier based on a GUID.</summary>
        /// <returns>A new instance of the <see cref="JsonRpcId" /> type.</returns>
        protected static JsonRpcId GenerateRequestId()
        {
            return new JsonRpcId(Guid.NewGuid().ToString());
        }

        /// <summary>Sends the specified JSON-RPC request as an asynchronous operation.</summary>
        /// <param name="request">The JSON-RPC request to send.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the JSON-RPC response.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="request" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcClientException">An error occurred during processing JSON-RPC method parameters, result, or error data.</exception>
        /// <exception cref="JsonRpcProtocolException">An error occurred during communication with a JSON-RPC service.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during invocation of a JSON-RPC service method.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        protected async Task<JsonRpcResponse> SendJsonRpcRequestAsync(JsonRpcRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var requestId = request.Id;

            using (var requestStream = new MemoryStream(_messageBufferSize))
            {
                try
                {
                    _jsonRpcSerializer.SerializeRequest(request, requestStream);
                }
                catch (JsonException e)
                {
                    throw new JsonRpcClientException(Strings.GetString("invoke.params.invalid_values"), requestId, e);
                }
                catch (JsonRpcException e)
                {
                    throw new JsonRpcClientException(Strings.GetString("invoke.params.invalid_values"), requestId, e);
                }

                cancellationToken.ThrowIfCancellationRequested();
                requestStream.Position = 0;

                using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, _serviceUri))
                {
                    PrepareHttpRequest(httpRequest);

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
                                    if (requestId.Type == JsonRpcIdType.None)
                                    {
                                        throw new JsonRpcProtocolException(httpResponse.StatusCode, Strings.GetString("protocol.service.message.unexpected_content"), requestId);
                                    }

                                    var contentType = httpResponse.Content.Headers.ContentType;

                                    if (contentType == null)
                                    {
                                        throw new JsonRpcProtocolException(httpResponse.StatusCode, Strings.GetString("protocol.http.headers.content_type.missing_value"));
                                    }
                                    if (string.Compare(contentType.MediaType, _mediaTypeValue.MediaType, StringComparison.OrdinalIgnoreCase) != 0)
                                    {
                                        throw new JsonRpcProtocolException(httpResponse.StatusCode, Strings.GetString("protocol.http.headers.content_type.invalid_value"));
                                    }

                                    var responseDataInfo = default(JsonRpcInfo<JsonRpcResponse>);
                                    var responseStream = default(Stream);

                                    using (responseStream = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false))
                                    {
                                        cancellationToken.ThrowIfCancellationRequested();

#if NETCOREAPP2_1

                                        if (CheckHttpContentEncoding(httpResponse, _brotliEncodingHeader.Value))
                                        {
                                            responseStream = new BrotliStream(responseStream, CompressionMode.Decompress);
                                        }

#endif

                                        try
                                        {
                                            responseDataInfo = await _jsonRpcSerializer.DeserializeResponseDataAsync(responseStream, cancellationToken).ConfigureAwait(false);
                                        }
                                        catch (JsonException e)
                                        {
                                            throw new JsonRpcClientException(Strings.GetString("protocol.rpc.message.invalid_value"), requestId, e);
                                        }
                                        catch (JsonRpcException e)
                                        {
                                            throw new JsonRpcClientException(Strings.GetString("protocol.rpc.message.invalid_value"), requestId, e);
                                        }
                                    }

                                    if (responseDataInfo.IsBatch)
                                    {
                                        throw new JsonRpcProtocolException(httpResponse.StatusCode, Strings.GetString("protocol.service.message.batch_value"), requestId);
                                    }

                                    var responseInfo = responseDataInfo.Message;

                                    if (!responseInfo.IsValid)
                                    {
                                        throw new JsonRpcClientException(Strings.GetString("protocol.service.message.invalid_value"), requestId, responseInfo.Exception);
                                    }

                                    var response = responseInfo.Message;

                                    if (!response.Success)
                                    {
                                        throw new JsonRpcServiceException(response.Error.Code, response.Error.Message, response.Error.Data, response.Error.HasData);
                                    }

                                    return response;
                                }
                            case HttpStatusCode.NoContent:
                                {
                                    if (requestId.Type != JsonRpcIdType.None)
                                    {
                                        throw new JsonRpcProtocolException(httpResponse.StatusCode, Strings.GetString("protocol.service.message.unexpected_blank"), requestId);
                                    }

                                    return null;
                                }
                            default:
                                {
                                    throw new JsonRpcProtocolException(httpResponse.StatusCode, Strings.GetString("protocol.http.status_code.invalid_value"));
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
        /// <exception cref="AggregateException">An error occurred during invocation of a JSON-RPC service method.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="requests" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcClientException">An error occurred during processing JSON-RPC method parameters, result, or error data.</exception>
        /// <exception cref="JsonRpcProtocolException">An error occurred during communication with a JSON-RPC service.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during invocation of a JSON-RPC service method.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        protected async Task<IReadOnlyList<JsonRpcResponse>> SendJsonRpcRequestsAsync(IReadOnlyList<JsonRpcRequest> requests, CancellationToken cancellationToken = default)
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
                    throw new JsonRpcClientException(Strings.GetString("invoke.batch.duplicate_identifiers"));
                }
            }

            using (var requestStream = new MemoryStream(_messageBufferSize * requests.Count))
            {
                try
                {
                    _jsonRpcSerializer.SerializeRequests(requests, requestStream);
                }
                catch (JsonException e)
                {
                    throw new JsonRpcClientException(Strings.GetString("invoke.params.invalid_values"), default, e);
                }
                catch (JsonRpcException e)
                {
                    throw new JsonRpcClientException(Strings.GetString("invoke.params.invalid_values"), default, e);
                }

                cancellationToken.ThrowIfCancellationRequested();
                requestStream.Position = 0;

                using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, _serviceUri))
                {
                    PrepareHttpRequest(httpRequest);

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
                                        throw new JsonRpcProtocolException(httpResponse.StatusCode, Strings.GetString("protocol.http.headers.content_type.missing_value"));
                                    }
                                    if (string.Compare(contentType.MediaType, _mediaTypeValue.MediaType, StringComparison.OrdinalIgnoreCase) != 0)
                                    {
                                        throw new JsonRpcProtocolException(httpResponse.StatusCode, Strings.GetString("protocol.http.headers.content_type.invalid_value"));
                                    }

                                    var responseDataInfo = default(JsonRpcInfo<JsonRpcResponse>);
                                    var responseStream = default(Stream);

                                    using (responseStream = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false))
                                    {
                                        cancellationToken.ThrowIfCancellationRequested();

#if NETCOREAPP2_1

                                        if (CheckHttpContentEncoding(httpResponse, _brotliEncodingHeader.Value))
                                        {
                                            responseStream = new BrotliStream(responseStream, CompressionMode.Decompress);
                                        }

#endif

                                        try
                                        {
                                            responseDataInfo = await _jsonRpcSerializer.DeserializeResponseDataAsync(responseStream, cancellationToken).ConfigureAwait(false);
                                        }
                                        catch (JsonException e)
                                        {
                                            throw new JsonRpcClientException(Strings.GetString("protocol.rpc.message.invalid_value"), default, e);
                                        }
                                        catch (JsonRpcException e)
                                        {
                                            throw new JsonRpcClientException(Strings.GetString("protocol.rpc.message.invalid_value"), default, e);
                                        }
                                    }

                                    if (responseDataInfo.IsBatch)
                                    {
                                        var responseInfos = responseDataInfo.Messages;
                                        var responseIdentifiers = new HashSet<JsonRpcId>();
                                        var responses = new JsonRpcResponse[responseInfos.Count];
                                        var exceptions = new HashSet<JsonRpcException>();

                                        for (var i = 0; i < responseInfos.Count; i++)
                                        {
                                            var responseInfo = responseInfos[i];

                                            if (responseInfo.IsValid)
                                            {
                                                var response = responseInfo.Message;

                                                responses[i] = response;

                                                if (!responseIdentifiers.Add(response.Id))
                                                {
                                                    throw new JsonRpcProtocolException(httpResponse.StatusCode, Strings.GetString("protocol.service.message.duplicate_identifiers"));
                                                }
                                            }
                                            else
                                            {
                                                exceptions.Add(new JsonRpcClientException(Strings.GetString("protocol.service.message.invalid_value"), default, responseInfo.Exception));
                                            }
                                        }

                                        if (exceptions.Count != 0)
                                        {
                                            throw new AggregateException(Strings.GetString("protocol.service.message.invalid_value"), exceptions);
                                        }
                                        if (!requestIdentifiers.SetEquals(responseIdentifiers))
                                        {
                                            throw new JsonRpcProtocolException(httpResponse.StatusCode, Strings.GetString("protocol.service.message.invalid_values"));
                                        }

                                        return responses;
                                    }
                                    else
                                    {
                                        var responseInfo = responseDataInfo.Message;

                                        if (!responseInfo.IsValid)
                                        {
                                            throw new JsonRpcClientException(Strings.GetString("protocol.service.message.invalid_value"), default, responseInfo.Exception);
                                        }

                                        var response = responseInfo.Message;

                                        if (!response.Success)
                                        {
                                            throw new JsonRpcServiceException(response.Error.Code, response.Error.Message, response.Error.Data, response.Error.HasData);
                                        }
                                        else
                                        {
                                            throw new JsonRpcProtocolException(httpResponse.StatusCode, Strings.GetString("protocol.service.message.single_value"));
                                        }
                                    }
                                }
                            case HttpStatusCode.NoContent:
                                {
                                    if (requestIdentifiers.Count != 0)
                                    {
                                        throw new JsonRpcProtocolException(httpResponse.StatusCode, Strings.GetString("protocol.service.message.invalid_values"));
                                    }

#if NETSTANDARD1_1

                                    return new JsonRpcResponse[] { };

#else

                                    return Array.Empty<JsonRpcResponse>();

#endif

                                }
                            default:
                                {
                                    throw new JsonRpcProtocolException(httpResponse.StatusCode, Strings.GetString("protocol.http.status_code.invalid_value"));
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

        /// <summary>Gets the current JSON-RPC message contract resolver.</summary>
        protected JsonRpcContractResolver ContractResolver
        {
            get => _jsonRpcContractResolver;
        }

        /// <summary>Gets the current service URI.</summary>
        protected Uri ServiceUri
        {
            get => _serviceUri;
        }
    }
}