// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Anemonis.JsonRpc.ServiceClient.Resources;

using Newtonsoft.Json;

#if NETCOREAPP2_1

using System.IO.Compression;

#endif

namespace Anemonis.JsonRpc.ServiceClient
{
    public partial class JsonRpcClient
    {
        private const int _streamBufferSize = 1024;
        private const int _messageBufferSize = 64;

        private static readonly IReadOnlyDictionary<string, Encoding> _supportedEncodings = CreateSupportedEncodings();
        private static readonly MediaTypeHeaderValue _mediaTypeHeaderValue = MediaTypeWithQualityHeaderValue.Parse("application/json; charset=utf-8");
        private static readonly MediaTypeWithQualityHeaderValue _mediaTypeWithQualityHeaderValue = MediaTypeWithQualityHeaderValue.Parse("application/json; charset=utf-8");

#if NETCOREAPP2_1

        private static readonly StringWithQualityHeaderValue _brotliEncodingHeaderValue = new StringWithQualityHeaderValue("br");

#endif

        private readonly JsonRpcContractResolver _jsonRpcContractResolver = new JsonRpcContractResolver();
        private readonly JsonRpcSerializer _jsonRpcSerializer;
        private readonly Uri _serviceUri;
        private readonly HttpMessageInvoker _httpInvoker;

        private static JsonSerializer CreateJsonSerializer()
        {
            var settings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore
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

        private static void ValidateUriScheme(string scheme)
        {
            if ((string.Compare(scheme, "HTTP", StringComparison.OrdinalIgnoreCase) != 0) &&
                (string.Compare(scheme, "HTTPS", StringComparison.OrdinalIgnoreCase) != 0))
            {
                throw new FormatException(Strings.GetString("client.uri.invalid_format"));
            }
        }

        private static IReadOnlyDictionary<string, Encoding> CreateSupportedEncodings()
        {
            return new Dictionary<string, Encoding>(StringComparer.OrdinalIgnoreCase)
            {

#if NETSTANDARD1_1

                [Encoding.UTF8.WebName] = new UTF8Encoding(false, true),
                [Encoding.Unicode.WebName] = new UnicodeEncoding(false, false, true)

#else

                [Encoding.UTF8.WebName] = new UTF8Encoding(false, true),
                [Encoding.Unicode.WebName] = new UnicodeEncoding(false, false, true),
                [Encoding.UTF32.WebName] = new UTF32Encoding(false, false, true)

#endif
            };
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

            if (!httpRequest.Headers.AcceptEncoding.Contains(_brotliEncodingHeaderValue))
            {
                httpRequest.Headers.AcceptEncoding.Add(_brotliEncodingHeaderValue);
            }

#endif

            httpRequest.Headers.Accept.Clear();
            httpRequest.Headers.Accept.Add(_mediaTypeWithQualityHeaderValue);
        }

        private async Task<JsonRpcResponse> SendJsonRpcRequestAsync(JsonRpcRequest request, JsonRpcResponseContract contract, CancellationToken cancellationToken)
        {
            var requestId = request.Id;

            _jsonRpcContractResolver.AddResponseContract(requestId, contract);

            try
            {
                return await SendJsonRpcRequestAsync(request, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _jsonRpcContractResolver.RemoveResponseContract(requestId);
            }
        }

        private JsonRpcId GetVerifiedRequestId()
        {
            var requestId = GetUniqueRequestId();

            if (requestId.Type == JsonRpcIdType.None)
            {
                throw new InvalidOperationException(Strings.GetString("invoke.identifier.invalid_value"));
            }

            return requestId;
        }

        /// <summary>Gets a new unique JSON-RPC request identifier.</summary>
        /// <returns>An instance of the <see cref="JsonRpcId" /> type.</returns>
        protected virtual JsonRpcId GetUniqueRequestId()
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

                    requestContent.Headers.ContentType = _mediaTypeHeaderValue;
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

                                    var contentTypeHeaderValue = httpResponse.Content.Headers.ContentType;

                                    if (contentTypeHeaderValue == null)
                                    {
                                        throw new JsonRpcProtocolException(httpResponse.StatusCode, Strings.GetString("protocol.http.headers.content_type.invalid_value"));
                                    }
                                    if (!contentTypeHeaderValue.MediaType.Equals(_mediaTypeHeaderValue.MediaType, StringComparison.OrdinalIgnoreCase))
                                    {
                                        throw new JsonRpcProtocolException(httpResponse.StatusCode, Strings.GetString("protocol.http.headers.content_type.invalid_value"));
                                    }
                                    if (!_supportedEncodings.TryGetValue(contentTypeHeaderValue.CharSet ?? Encoding.UTF8.WebName, out var responseEncoding))
                                    {
                                        throw new JsonRpcProtocolException(httpResponse.StatusCode, Strings.GetString("protocol.http.headers.content_type.invalid_value"));
                                    }

                                    var responseData = default(JsonRpcData<JsonRpcResponse>);
                                    var responseStream = default(Stream);

                                    try
                                    {
                                        responseStream = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
                                        cancellationToken.ThrowIfCancellationRequested();

#if NETCOREAPP2_1

                                        if (CheckHttpContentEncoding(httpResponse, _brotliEncodingHeaderValue.Value))
                                        {
                                            responseStream = new BrotliStream(responseStream, CompressionMode.Decompress);
                                        }

#endif

                                        try
                                        {
                                            using (var streamReader = new StreamReader(responseStream, responseEncoding, false, _streamBufferSize, true))
                                            {
                                                responseData = await _jsonRpcSerializer.DeserializeResponseDataAsync(streamReader, cancellationToken).ConfigureAwait(false);
                                            }
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
                                    finally
                                    {
                                        responseStream?.Dispose();
                                    }

                                    if (responseData.IsBatch)
                                    {
                                        throw new JsonRpcProtocolException(httpResponse.StatusCode, Strings.GetString("protocol.service.message.batch_value"), requestId);
                                    }

                                    var responseItem = responseData.Item;

                                    if (!responseItem.IsValid)
                                    {
                                        throw new JsonRpcClientException(Strings.GetString("protocol.service.message.invalid_value"), requestId, responseItem.Exception);
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

                    requestContent.Headers.ContentType = _mediaTypeHeaderValue;
                    httpRequest.Content = requestContent;

                    using (var httpResponse = await _httpInvoker.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false))
                    {
                        VisitHttpResponseHeaders(httpResponse.Headers);

                        switch (httpResponse.StatusCode)
                        {
                            case HttpStatusCode.OK:
                                {
                                    var contentTypeHeaderValue = httpResponse.Content.Headers.ContentType;

                                    if (contentTypeHeaderValue == null)
                                    {
                                        throw new JsonRpcProtocolException(httpResponse.StatusCode, Strings.GetString("protocol.http.headers.content_type.invalid_value"));
                                    }
                                    if (!contentTypeHeaderValue.MediaType.Equals(_mediaTypeHeaderValue.MediaType, StringComparison.OrdinalIgnoreCase))
                                    {
                                        throw new JsonRpcProtocolException(httpResponse.StatusCode, Strings.GetString("protocol.http.headers.content_type.invalid_value"));
                                    }
                                    if (!_supportedEncodings.TryGetValue(contentTypeHeaderValue.CharSet ?? Encoding.UTF8.WebName, out var responseEncoding))
                                    {
                                        throw new JsonRpcProtocolException(httpResponse.StatusCode, Strings.GetString("protocol.http.headers.content_type.invalid_value"));
                                    }

                                    var responseData = default(JsonRpcData<JsonRpcResponse>);
                                    var responseStream = default(Stream);

                                    try
                                    {
                                        responseStream = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
                                        cancellationToken.ThrowIfCancellationRequested();

#if NETCOREAPP2_1

                                        if (CheckHttpContentEncoding(httpResponse, _brotliEncodingHeaderValue.Value))
                                        {
                                            responseStream = new BrotliStream(responseStream, CompressionMode.Decompress);
                                        }

#endif

                                        try
                                        {
                                            using (var streamReader = new StreamReader(responseStream, responseEncoding, false, _streamBufferSize, true))
                                            {
                                                responseData = await _jsonRpcSerializer.DeserializeResponseDataAsync(streamReader, cancellationToken).ConfigureAwait(false);
                                            }
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
                                    finally
                                    {
                                        responseStream?.Dispose();
                                    }

                                    if (responseData.IsBatch)
                                    {
                                        var responseInfos = responseData.Items;
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
                                        var responseItem = responseData.Item;

                                        if (!responseItem.IsValid)
                                        {
                                            throw new JsonRpcClientException(Strings.GetString("protocol.service.message.invalid_value"), default, responseItem.Exception);
                                        }

                                        var response = responseItem.Message;

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
