// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Anemonis.JsonRpc.ServiceClient.Resources;

using Newtonsoft.Json;

#pragma warning disable CA2000

namespace Anemonis.JsonRpc.ServiceClient
{
    public partial class JsonRpcClient
    {
        private const int _messageBufferSize = 64;

        private static readonly IReadOnlyDictionary<string, Encoding> _supportedEncodings = CreateSupportedEncodings();
        private static readonly MediaTypeHeaderValue _mediaTypeHeaderValue = MediaTypeWithQualityHeaderValue.Parse("application/json; charset=utf-8");
        private static readonly MediaTypeWithQualityHeaderValue _mediaTypeWithQualityHeaderValue = MediaTypeWithQualityHeaderValue.Parse("application/json; charset=utf-8");
        private static readonly StringWithQualityHeaderValue _brotliEncodingHeaderValue = new StringWithQualityHeaderValue("br");

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
            if ((string.Compare(scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) != 0) &&
                (string.Compare(scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) != 0))
            {
                throw new FormatException(Strings.GetString("client.uri.invalid_format"));
            }
        }

        private static IReadOnlyDictionary<string, Encoding> CreateSupportedEncodings()
        {
            return new Dictionary<string, Encoding>(StringComparer.OrdinalIgnoreCase)
            {
                [Encoding.UTF8.WebName] = new UTF8Encoding(false, true),
                [Encoding.Unicode.WebName] = new UnicodeEncoding(false, false, true),
                [Encoding.UTF32.WebName] = new UTF32Encoding(false, false, true)
            };
        }

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

        private void InternalVisitHttpRequestMessage(HttpRequestMessage message)
        {
            VisitHttpRequestMessage(message);

            if (message.Content == null)
            {
                throw new InvalidOperationException(Strings.GetString("client.http_request_content.invalid_value"));
            }
            if (!message.Headers.AcceptEncoding.Contains(_brotliEncodingHeaderValue))
            {
                message.Headers.AcceptEncoding.Add(_brotliEncodingHeaderValue);
            }

            message.Headers.Accept.Clear();
            message.Headers.Accept.Add(_mediaTypeWithQualityHeaderValue);
            message.Content.Headers.ContentType = _mediaTypeHeaderValue;
        }

        private void InternalVisitHttpResponseMessage(HttpResponseMessage message, out Encoding encoding)
        {
            if (message.Content != null)
            {
                var contentTypeHeaderValue = message.Content.Headers.ContentType;

                if (contentTypeHeaderValue == null)
                {
                    throw new JsonRpcProtocolException(message.StatusCode, Strings.GetString("protocol.http.headers.content_type.invalid_value"));
                }
                if (!contentTypeHeaderValue.MediaType.Equals(_mediaTypeHeaderValue.MediaType, StringComparison.OrdinalIgnoreCase))
                {
                    throw new JsonRpcProtocolException(message.StatusCode, Strings.GetString("protocol.http.headers.content_type.invalid_value"));
                }
                if (!_supportedEncodings.TryGetValue(contentTypeHeaderValue.CharSet ?? Encoding.UTF8.WebName, out encoding))
                {
                    throw new JsonRpcProtocolException(message.StatusCode, Strings.GetString("protocol.http.headers.content_type.invalid_value"));
                }
            }
            else
            {
                encoding = null;
            }

            VisitHttpResponseMessage(message);
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
                requestStream.Position = 0L;

                using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, _serviceUri))
                {
                    httpRequest.Content = new StreamContent(requestStream);

                    InternalVisitHttpRequestMessage(httpRequest);

                    requestStream.Position = 0L;

                    using (var httpResponse = await _httpInvoker.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false))
                    {
                        switch (httpResponse.StatusCode)
                        {
                            case HttpStatusCode.OK:
                                {
                                    InternalVisitHttpResponseMessage(httpResponse, out var responseEncoding);

                                    if (requestId.Type == JsonRpcIdType.None)
                                    {
                                        throw new JsonRpcProtocolException(httpResponse.StatusCode, Strings.GetString("protocol.service.message.unexpected_content"), requestId);
                                    }

                                    var responseData = default(JsonRpcData<JsonRpcResponse>);
                                    var responseStream = default(Stream);

                                    try
                                    {
                                        responseStream = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
                                        cancellationToken.ThrowIfCancellationRequested();

                                        if (CheckHttpContentEncoding(httpResponse, _brotliEncodingHeaderValue.Value))
                                        {
                                            responseStream = new BrotliStream(responseStream, CompressionMode.Decompress);
                                        }

                                        try
                                        {
                                            responseData = await _jsonRpcSerializer.DeserializeResponseDataAsync(responseStream, responseEncoding, cancellationToken).ConfigureAwait(false);
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
                                    InternalVisitHttpResponseMessage(httpResponse, out _);

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
                requestStream.Position = 0L;

                using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, _serviceUri))
                {
                    httpRequest.Content = new StreamContent(requestStream);

                    InternalVisitHttpRequestMessage(httpRequest);

                    requestStream.Position = 0L;

                    using (var httpResponse = await _httpInvoker.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false))
                    {
                        switch (httpResponse.StatusCode)
                        {
                            case HttpStatusCode.OK:
                                {
                                    InternalVisitHttpResponseMessage(httpResponse, out var responseEncoding);

                                    var responseData = default(JsonRpcData<JsonRpcResponse>);
                                    var responseStream = default(Stream);

                                    try
                                    {
                                        responseStream = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
                                        cancellationToken.ThrowIfCancellationRequested();

                                        if (CheckHttpContentEncoding(httpResponse, _brotliEncodingHeaderValue.Value))
                                        {
                                            responseStream = new BrotliStream(responseStream, CompressionMode.Decompress);
                                        }

                                        try
                                        {
                                            responseData = await _jsonRpcSerializer.DeserializeResponseDataAsync(responseStream, responseEncoding, cancellationToken).ConfigureAwait(false);
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
                                    InternalVisitHttpResponseMessage(httpResponse, out _);

                                    if (requestIdentifiers.Count != 0)
                                    {
                                        throw new JsonRpcProtocolException(httpResponse.StatusCode, Strings.GetString("protocol.service.message.invalid_values"));
                                    }

                                    return Array.Empty<JsonRpcResponse>();
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

        /// <summary>Visits an HTTP request message.</summary>
        /// <param name="message">The HTTP request message.</param>
        protected virtual void VisitHttpRequestMessage(HttpRequestMessage message)
        {
        }

        /// <summary>Visits an HTTP response message.</summary>
        /// <param name="headers">The HTTP response message.</param>
        protected virtual void VisitHttpResponseMessage(HttpResponseMessage headers)
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
