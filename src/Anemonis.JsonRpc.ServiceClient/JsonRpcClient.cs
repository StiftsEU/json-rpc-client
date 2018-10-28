// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Anemonis.JsonRpc.ServiceClient.Resources;
using Newtonsoft.Json;

namespace Anemonis.JsonRpc.ServiceClient
{
    /// <summary>Represents a JSON-RPC 2.0 service client.</summary>
    public partial class JsonRpcClient : IDisposable
    {
        /// <summary>Initializes a new instance of the <see cref="JsonRpcClient" /> class.</summary>
        /// <param name="serviceUri">The service URI.</param>
        /// <param name="jsonSerializer">The component for serializing to and deserializing from JSON.</param>
        /// <param name="httpInvoker">The component for HTTP/HTTPS communication.</param>
        /// <param name="compatibilityLevel">The JSON-RPC protocol compatibility level.</param>
        /// <exception cref="ArgumentNullException"><paramref name="serviceUri" />, <paramref name="jsonSerializer" />, or <paramref name="httpInvoker" /> is <see langword="null" />.</exception>
        /// <exception cref="FormatException"><paramref name="serviceUri" /> is relative, or has invalid scheme, or is not correctly formed.</exception>
        public JsonRpcClient(string serviceUri, JsonSerializer jsonSerializer, HttpMessageInvoker httpInvoker, JsonRpcCompatibilityLevel compatibilityLevel = default)
        {
            if (serviceUri == null)
            {
                throw new ArgumentNullException(nameof(serviceUri));
            }
            if (jsonSerializer == null)
            {
                throw new ArgumentNullException(nameof(jsonSerializer));
            }
            if (httpInvoker == null)
            {
                throw new ArgumentNullException(nameof(httpInvoker));
            }
            if (!Uri.IsWellFormedUriString(serviceUri, UriKind.Absolute))
            {
                throw new FormatException(Strings.GetString("client.uri.invalid_format"));
            }

            _serviceUri = new Uri(serviceUri, UriKind.Absolute);
            _httpInvoker = httpInvoker;
            _jsonRpcSerializer = new JsonRpcSerializer(_jsonRpcContractResolver, jsonSerializer, compatibilityLevel);

            ValidateUriScheme(_serviceUri.Scheme);
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcClient" /> class.</summary>
        /// <param name="serviceUri">The service URI.</param>
        /// <param name="jsonSerializer">The component for serializing to and deserializing from JSON.</param>
        /// <param name="compatibilityLevel">The JSON-RPC protocol compatibility level.</param>
        /// <exception cref="ArgumentNullException"><paramref name="serviceUri" /> or <paramref name="jsonSerializer" /> is <see langword="null" />.</exception>
        /// <exception cref="FormatException"><paramref name="serviceUri" /> is relative, or has invalid scheme, or is not correctly formed.</exception>
        public JsonRpcClient(string serviceUri, JsonSerializer jsonSerializer, JsonRpcCompatibilityLevel compatibilityLevel = default)
            : this(serviceUri, jsonSerializer, CreateHttpInvoker(), compatibilityLevel)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcClient" /> class.</summary>
        /// <param name="serviceUri">The service URI.</param>
        /// <param name="httpInvoker">The component for HTTP/HTTPS communication.</param>
        /// <param name="compatibilityLevel">The JSON-RPC protocol compatibility level.</param>
        /// <exception cref="ArgumentNullException"><paramref name="serviceUri" /> or <paramref name="httpInvoker" /> is <see langword="null" />.</exception>
        /// <exception cref="FormatException"><paramref name="serviceUri" /> is relative, or has invalid scheme, or is not correctly formed.</exception>
        public JsonRpcClient(string serviceUri, HttpMessageInvoker httpInvoker, JsonRpcCompatibilityLevel compatibilityLevel = default)
            : this(serviceUri, CreateJsonSerializer(), httpInvoker, compatibilityLevel)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcClient" /> class.</summary>
        /// <param name="serviceUri">The service URI.</param>
        /// <param name="compatibilityLevel">The JSON-RPC protocol compatibility level.</param>
        /// <exception cref="ArgumentNullException"><paramref name="serviceUri" /> is <see langword="null" />.</exception>
        /// <exception cref="FormatException"><paramref name="serviceUri" /> is relative, or has invalid scheme, or is not correctly formed.</exception>
        public JsonRpcClient(string serviceUri, JsonRpcCompatibilityLevel compatibilityLevel = default)
            : this(serviceUri, CreateJsonSerializer(), CreateHttpInvoker(), compatibilityLevel)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcClient" /> class.</summary>
        /// <param name="serviceUri">The service URI.</param>
        /// <param name="jsonSerializer">The component for serializing to and deserializing from JSON.</param>
        /// <param name="httpInvoker">The component for HTTP/HTTPS communication.</param>
        /// <param name="compatibilityLevel">The JSON-RPC protocol compatibility level.</param>
        /// <exception cref="ArgumentException"><paramref name="serviceUri" /> is a relative URI.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="serviceUri" />, <paramref name="jsonSerializer" />, or <paramref name="httpInvoker" /> is <see langword="null" />.</exception>
        public JsonRpcClient(Uri serviceUri, JsonSerializer jsonSerializer, HttpMessageInvoker httpInvoker, JsonRpcCompatibilityLevel compatibilityLevel = default)
        {
            if (serviceUri == null)
            {
                throw new ArgumentNullException(nameof(serviceUri));
            }
            if (jsonSerializer == null)
            {
                throw new ArgumentNullException(nameof(jsonSerializer));
            }
            if (httpInvoker == null)
            {
                throw new ArgumentNullException(nameof(httpInvoker));
            }
            if (!serviceUri.IsAbsoluteUri)
            {
                throw new FormatException(Strings.GetString("client.uri.invalid_format"));
            }

            _serviceUri = serviceUri;
            _httpInvoker = httpInvoker;
            _jsonRpcSerializer = new JsonRpcSerializer(_jsonRpcContractResolver, jsonSerializer, compatibilityLevel);

            ValidateUriScheme(_serviceUri.Scheme);
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcClient" /> class.</summary>
        /// <param name="serviceUri">The service URI.</param>
        /// <param name="jsonSerializer">The component for serializing to and deserializing from JSON.</param>
        /// <param name="compatibilityLevel">The JSON-RPC protocol compatibility level.</param>
        /// <exception cref="ArgumentNullException"><paramref name="serviceUri" /> or <paramref name="jsonSerializer" /> is <see langword="null" />.</exception>
        /// <exception cref="FormatException"><paramref name="serviceUri" /> is relative, or has invalid scheme, or is not correctly formed.</exception>
        public JsonRpcClient(Uri serviceUri, JsonSerializer jsonSerializer, JsonRpcCompatibilityLevel compatibilityLevel = default)
            : this(serviceUri, jsonSerializer, CreateHttpInvoker(), compatibilityLevel)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcClient" /> class.</summary>
        /// <param name="serviceUri">The service URI.</param>
        /// <param name="httpInvoker">The component for HTTP/HTTPS communication.</param>
        /// <param name="compatibilityLevel">The JSON-RPC protocol compatibility level.</param>
        /// <exception cref="ArgumentNullException"><paramref name="serviceUri" /> or <paramref name="httpInvoker" /> is <see langword="null" />.</exception>
        /// <exception cref="FormatException"><paramref name="serviceUri" /> is relative, or has invalid scheme, or is not correctly formed.</exception>
        public JsonRpcClient(Uri serviceUri, HttpMessageInvoker httpInvoker, JsonRpcCompatibilityLevel compatibilityLevel = default)
            : this(serviceUri, CreateJsonSerializer(), httpInvoker, compatibilityLevel)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcClient" /> class.</summary>
        /// <param name="serviceUri">The service URI.</param>
        /// <param name="compatibilityLevel">The JSON-RPC protocol compatibility level.</param>
        /// <exception cref="ArgumentException"><paramref name="serviceUri" /> is a relative URI.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="serviceUri" /> is <see langword="null" />.</exception>
        public JsonRpcClient(Uri serviceUri, JsonRpcCompatibilityLevel compatibilityLevel = default)
            : this(serviceUri, CreateJsonSerializer(), CreateHttpInvoker(), compatibilityLevel)
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
        /// <exception cref="JsonRpcClientException">An error occurred during processing JSON-RPC method parameters, result, or error data.</exception>
        /// <exception cref="JsonRpcProtocolException">An error occurred during communication with a JSON-RPC service.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during invocation of a JSON-RPC service method.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public Task InvokeAsync(string method, CancellationToken cancellationToken = default)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (JsonRpcProtocol.IsSystemMethod(method))
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
        /// <exception cref="JsonRpcClientException">An error occurred during processing JSON-RPC method parameters, result, or error data.</exception>
        /// <exception cref="JsonRpcProtocolException">An error occurred during communication with a JSON-RPC service.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during invocation of a JSON-RPC service method.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public Task InvokeAsync(string method, IReadOnlyList<object> parameters, CancellationToken cancellationToken = default)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (JsonRpcProtocol.IsSystemMethod(method))
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
        /// <exception cref="JsonRpcClientException">An error occurred during processing JSON-RPC method parameters, result, or error data.</exception>
        /// <exception cref="JsonRpcProtocolException">An error occurred during communication with a JSON-RPC service.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during invocation of a JSON-RPC service method.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public Task InvokeAsync(string method, IReadOnlyDictionary<string, object> parameters, CancellationToken cancellationToken = default)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (JsonRpcProtocol.IsSystemMethod(method))
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
        /// <exception cref="JsonRpcClientException">An error occurred during processing JSON-RPC method parameters, result, or error data.</exception>
        /// <exception cref="JsonRpcProtocolException">An error occurred during communication with a JSON-RPC service.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during invocation of a JSON-RPC service method.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<TResult> InvokeAsync<TResult>(string method, CancellationToken cancellationToken = default)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (JsonRpcProtocol.IsSystemMethod(method))
            {
                throw new ArgumentException(Strings.GetString("invoke.method.invalid_name"), nameof(method));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var request = new JsonRpcRequest(method, GetVerifiedRequestId());
            var response = await SendJsonRpcRequestAsync(request, JsonRpcResponseContract<TResult>.Instance, cancellationToken).ConfigureAwait(false);

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
        /// <exception cref="JsonRpcClientException">An error occurred during processing JSON-RPC method parameters, result, or error data.</exception>
        /// <exception cref="JsonRpcProtocolException">An error occurred during communication with a JSON-RPC service.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during invocation of a JSON-RPC service method.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<TResult> InvokeAsync<TResult>(string method, IReadOnlyList<object> parameters, CancellationToken cancellationToken = default)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (JsonRpcProtocol.IsSystemMethod(method))
            {
                throw new ArgumentException(Strings.GetString("invoke.method.invalid_name"), nameof(method));
            }
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var request = new JsonRpcRequest(method, GetVerifiedRequestId(), parameters);
            var response = await SendJsonRpcRequestAsync(request, JsonRpcResponseContract<TResult>.Instance, cancellationToken).ConfigureAwait(false);

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
        /// <exception cref="JsonRpcClientException">An error occurred during processing JSON-RPC method parameters, result, or error data.</exception>
        /// <exception cref="JsonRpcProtocolException">An error occurred during communication with a JSON-RPC service.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during invocation of a JSON-RPC service method.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<TResult> InvokeAsync<TResult>(string method, IReadOnlyDictionary<string, object> parameters, CancellationToken cancellationToken = default)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (JsonRpcProtocol.IsSystemMethod(method))
            {
                throw new ArgumentException(Strings.GetString("invoke.method.invalid_name"), nameof(method));
            }
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var request = new JsonRpcRequest(method, GetVerifiedRequestId(), parameters);
            var response = await SendJsonRpcRequestAsync(request, JsonRpcResponseContract<TResult>.Instance, cancellationToken).ConfigureAwait(false);

            return (TResult)response.Result;
        }

        /// <summary>Invokes the specified service method as an asynchronous operation.</summary>
        /// <typeparam name="TResult">The type of the service method result.</typeparam>
        /// <param name="method">The name of the service method.</param>
        /// <param name="requestId">The identifier for the corresponding JSON-RPC request.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the service method result.</returns>
        /// <exception cref="ArgumentException"><paramref name="method" /> is a system extension method or <paramref name="requestId" /> is void.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcClientException">An error occurred during processing JSON-RPC method parameters, result, or error data.</exception>
        /// <exception cref="JsonRpcProtocolException">An error occurred during communication with a JSON-RPC service.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during invocation of a JSON-RPC service method.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<TResult> InvokeAsync<TResult>(string method, JsonRpcId requestId, CancellationToken cancellationToken = default)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (JsonRpcProtocol.IsSystemMethod(method))
            {
                throw new ArgumentException(Strings.GetString("invoke.method.invalid_name"), nameof(method));
            }
            if (requestId.Type == JsonRpcIdType.None)
            {
                throw new ArgumentException(Strings.GetString("invoke.identifier.invalid_value"), nameof(requestId));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var request = new JsonRpcRequest(method, requestId);
            var response = await SendJsonRpcRequestAsync(request, JsonRpcResponseContract<TResult>.Instance, cancellationToken).ConfigureAwait(false);

            return (TResult)response.Result;
        }

        /// <summary>Invokes the specified service method as an asynchronous operation.</summary>
        /// <typeparam name="TResult">The type of the service method result.</typeparam>
        /// <param name="method">The name of the service method.</param>
        /// <param name="requestId">The identifier for the corresponding JSON-RPC request.</param>
        /// <param name="parameters">The parameters to be used during the invocation of the service method, specified by position.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the service method result.</returns>
        /// <exception cref="ArgumentException"><paramref name="method" /> is a system extension method or <paramref name="requestId" /> is void.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> or <paramref name="parameters" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcClientException">An error occurred during processing JSON-RPC method parameters, result, or error data.</exception>
        /// <exception cref="JsonRpcProtocolException">An error occurred during communication with a JSON-RPC service.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during invocation of a JSON-RPC service method.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<TResult> InvokeAsync<TResult>(string method, JsonRpcId requestId, IReadOnlyList<object> parameters, CancellationToken cancellationToken = default)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (JsonRpcProtocol.IsSystemMethod(method))
            {
                throw new ArgumentException(Strings.GetString("invoke.method.invalid_name"), nameof(method));
            }
            if (requestId.Type == JsonRpcIdType.None)
            {
                throw new ArgumentException(Strings.GetString("invoke.identifier.invalid_value"), nameof(requestId));
            }
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var request = new JsonRpcRequest(method, requestId, parameters);
            var response = await SendJsonRpcRequestAsync(request, JsonRpcResponseContract<TResult>.Instance, cancellationToken).ConfigureAwait(false);

            return (TResult)response.Result;
        }

        /// <summary>Invokes the specified service method as an asynchronous operation.</summary>
        /// <typeparam name="TResult">The type of the service method result.</typeparam>
        /// <param name="method">The name of the service method.</param>
        /// <param name="requestId">The identifier for the corresponding JSON-RPC request.</param>
        /// <param name="parameters">The parameters to be used during the invocation of the service method, specified by name.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the service method result.</returns>
        /// <exception cref="ArgumentException"><paramref name="method" /> is a system extension method or <paramref name="requestId" /> is void.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> or <paramref name="parameters" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcClientException">An error occurred during processing JSON-RPC method parameters, result, or error data.</exception>
        /// <exception cref="JsonRpcProtocolException">An error occurred during communication with a JSON-RPC service.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during invocation of a JSON-RPC service method.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<TResult> InvokeAsync<TResult>(string method, JsonRpcId requestId, IReadOnlyDictionary<string, object> parameters, CancellationToken cancellationToken = default)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (JsonRpcProtocol.IsSystemMethod(method))
            {
                throw new ArgumentException(Strings.GetString("invoke.method.invalid_name"), nameof(method));
            }
            if (requestId.Type == JsonRpcIdType.None)
            {
                throw new ArgumentException(Strings.GetString("invoke.identifier.invalid_value"), nameof(requestId));
            }
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var request = new JsonRpcRequest(method, requestId, parameters);
            var response = await SendJsonRpcRequestAsync(request, JsonRpcResponseContract<TResult>.Instance, cancellationToken).ConfigureAwait(false);

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
        /// <exception cref="JsonRpcClientException">An error occurred during processing JSON-RPC method parameters, result, or error data.</exception>
        /// <exception cref="JsonRpcProtocolException">An error occurred during communication with a JSON-RPC service.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during invocation of a JSON-RPC service method.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<TResult> InvokeAsync<TResult, TErrorData>(string method, CancellationToken cancellationToken = default)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (JsonRpcProtocol.IsSystemMethod(method))
            {
                throw new ArgumentException(Strings.GetString("invoke.method.invalid_name"), nameof(method));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var request = new JsonRpcRequest(method, GetVerifiedRequestId());
            var response = await SendJsonRpcRequestAsync(request, JsonRpcResponseContract<TResult, TErrorData>.Instance, cancellationToken).ConfigureAwait(false);

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
        /// <exception cref="JsonRpcClientException">An error occurred during processing JSON-RPC method parameters, result, or error data.</exception>
        /// <exception cref="JsonRpcProtocolException">An error occurred during communication with a JSON-RPC service.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during invocation of a JSON-RPC service method.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<TResult> InvokeAsync<TResult, TErrorData>(string method, IReadOnlyList<object> parameters, CancellationToken cancellationToken = default)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (JsonRpcProtocol.IsSystemMethod(method))
            {
                throw new ArgumentException(Strings.GetString("invoke.method.invalid_name"), nameof(method));
            }
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var request = new JsonRpcRequest(method, GetVerifiedRequestId(), parameters);
            var response = await SendJsonRpcRequestAsync(request, JsonRpcResponseContract<TResult>.Instance, cancellationToken).ConfigureAwait(false);

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
        /// <exception cref="JsonRpcClientException">An error occurred during processing JSON-RPC method parameters, result, or error data.</exception>
        /// <exception cref="JsonRpcProtocolException">An error occurred during communication with a JSON-RPC service.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during invocation of a JSON-RPC service method.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<TResult> InvokeAsync<TResult, TErrorData>(string method, IReadOnlyDictionary<string, object> parameters, CancellationToken cancellationToken = default)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (JsonRpcProtocol.IsSystemMethod(method))
            {
                throw new ArgumentException(Strings.GetString("invoke.method.invalid_name"), nameof(method));
            }
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var request = new JsonRpcRequest(method, GetVerifiedRequestId(), parameters);
            var response = await SendJsonRpcRequestAsync(request, JsonRpcResponseContract<TResult, TErrorData>.Instance, cancellationToken).ConfigureAwait(false);

            return (TResult)response.Result;
        }

        /// <summary>Invokes the specified service method as an asynchronous operation.</summary>
        /// <typeparam name="TResult">The type of the service method result.</typeparam>
        /// <typeparam name="TErrorData">The type of the service method optional error data.</typeparam>
        /// <param name="method">The name of the service method.</param>
        /// <param name="requestId">The identifier for the corresponding JSON-RPC request.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the service method result.</returns>
        /// <exception cref="ArgumentException"><paramref name="method" /> is a system extension method or <paramref name="requestId" /> is void.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcClientException">An error occurred during processing JSON-RPC method parameters, result, or error data.</exception>
        /// <exception cref="JsonRpcProtocolException">An error occurred during communication with a JSON-RPC service.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during invocation of a JSON-RPC service method.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<TResult> InvokeAsync<TResult, TErrorData>(string method, JsonRpcId requestId, CancellationToken cancellationToken = default)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (JsonRpcProtocol.IsSystemMethod(method))
            {
                throw new ArgumentException(Strings.GetString("invoke.method.invalid_name"), nameof(method));
            }
            if (requestId.Type == JsonRpcIdType.None)
            {
                throw new ArgumentException(Strings.GetString("invoke.identifier.invalid_value"), nameof(requestId));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var request = new JsonRpcRequest(method, requestId);
            var response = await SendJsonRpcRequestAsync(request, JsonRpcResponseContract<TResult, TErrorData>.Instance, cancellationToken).ConfigureAwait(false);

            return (TResult)response.Result;
        }

        /// <summary>Invokes the specified service method as an asynchronous operation.</summary>
        /// <typeparam name="TResult">The type of the service method result.</typeparam>
        /// <typeparam name="TErrorData">The type of the service method optional error data.</typeparam>
        /// <param name="method">The name of the service method.</param>
        /// <param name="requestId">The identifier for the corresponding JSON-RPC request.</param>
        /// <param name="parameters">The parameters to be used during the invocation of the service method, specified by position.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the service method result.</returns>
        /// <exception cref="ArgumentException"><paramref name="method" /> is a system extension method or <paramref name="requestId" /> is void.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> or <paramref name="parameters" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcClientException">An error occurred during processing JSON-RPC method parameters, result, or error data.</exception>
        /// <exception cref="JsonRpcProtocolException">An error occurred during communication with a JSON-RPC service.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during invocation of a JSON-RPC service method.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<TResult> InvokeAsync<TResult, TErrorData>(string method, JsonRpcId requestId, IReadOnlyList<object> parameters, CancellationToken cancellationToken = default)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (JsonRpcProtocol.IsSystemMethod(method))
            {
                throw new ArgumentException(Strings.GetString("invoke.method.invalid_name"), nameof(method));
            }
            if (requestId.Type == JsonRpcIdType.None)
            {
                throw new ArgumentException(Strings.GetString("invoke.identifier.invalid_value"), nameof(requestId));
            }
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var request = new JsonRpcRequest(method, requestId, parameters);
            var response = await SendJsonRpcRequestAsync(request, JsonRpcResponseContract<TResult, TErrorData>.Instance, cancellationToken).ConfigureAwait(false);

            return (TResult)response.Result;
        }

        /// <summary>Invokes the specified service method as an asynchronous operation.</summary>
        /// <typeparam name="TResult">The type of the service method result.</typeparam>
        /// <typeparam name="TErrorData">The type of the service method optional error data.</typeparam>
        /// <param name="method">The name of the service method.</param>
        /// <param name="requestId">The identifier for the corresponding JSON-RPC request.</param>
        /// <param name="parameters">The parameters to be used during the invocation of the service method, specified by name.</param>
        /// <param name="cancellationToken">The cancellation token for canceling the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the service method result.</returns>
        /// <exception cref="ArgumentException"><paramref name="method" /> is a system extension method or <paramref name="requestId" /> is void.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> or <paramref name="parameters" /> is <see langword="null" />.</exception>
        /// <exception cref="JsonRpcClientException">An error occurred during processing JSON-RPC method parameters, result, or error data.</exception>
        /// <exception cref="JsonRpcProtocolException">An error occurred during communication with a JSON-RPC service.</exception>
        /// <exception cref="JsonRpcServiceException">An error occurred during invocation of a JSON-RPC service method.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<TResult> InvokeAsync<TResult, TErrorData>(string method, JsonRpcId requestId, IReadOnlyDictionary<string, object> parameters, CancellationToken cancellationToken = default)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (JsonRpcProtocol.IsSystemMethod(method))
            {
                throw new ArgumentException(Strings.GetString("invoke.method.invalid_name"), nameof(method));
            }
            if (requestId.Type == JsonRpcIdType.None)
            {
                throw new ArgumentException(Strings.GetString("invoke.identifier.invalid_value"), nameof(requestId));
            }
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var request = new JsonRpcRequest(method, requestId, parameters);
            var response = await SendJsonRpcRequestAsync(request, JsonRpcResponseContract<TResult, TErrorData>.Instance, cancellationToken).ConfigureAwait(false);

            return (TResult)response.Result;
        }
    }
}