// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Anemonis.JsonRpc.ServiceClient
{
    internal abstract class JsonRpcClientProxy : IDisposable
    {
        private readonly JsonRpcClient _client;

        protected JsonRpcClientProxy(JsonRpcClient client)
        {
            _client = client;
        }

        public static object[] CreateParametersT1(int capacity)
        {
            return new object[capacity];
        }

        public static Dictionary<string, object> CreateParametersT2(int capacity)
        {
            return new Dictionary<string, object>(capacity, StringComparer.Ordinal);
        }

        public Task InvokeT000Async(string method)
        {
            return _client.InvokeAsync(method);
        }

        public Task InvokeT001Async(string method, CancellationToken cancellationToken)
        {
            return _client.InvokeAsync(method, cancellationToken);
        }

        public Task InvokeT010Async(string method, IReadOnlyList<object> parameters)
        {
            return _client.InvokeAsync(method, parameters);
        }

        public Task InvokeT011Async(string method, IReadOnlyList<object> parameters, CancellationToken cancellationToken)
        {
            return _client.InvokeAsync(method, parameters, cancellationToken);
        }

        public Task InvokeT020Async(string method, IReadOnlyDictionary<string, object> parameters)
        {
            return _client.InvokeAsync(method, parameters);
        }

        public Task InvokeT021Async(string method, IReadOnlyDictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            return _client.InvokeAsync(method, parameters, cancellationToken);
        }

        public Task<TResult> InvokeT100Async<TResult>(string method)
        {
            return _client.InvokeAsync<TResult>(method);
        }

        public Task<TResult> InvokeT101Async<TResult>(string method, CancellationToken cancellationToken)
        {
            return _client.InvokeAsync<TResult>(method, cancellationToken);
        }

        public Task<TResult> InvokeT110Async<TResult>(string method, IReadOnlyList<object> parameters)
        {
            return _client.InvokeAsync<TResult>(method, parameters);
        }

        public Task<TResult> InvokeT111Async<TResult>(string method, IReadOnlyList<object> parameters, CancellationToken cancellationToken)
        {
            return _client.InvokeAsync<TResult>(method, parameters, cancellationToken);
        }

        public Task<TResult> InvokeT120Async<TResult>(string method, IReadOnlyDictionary<string, object> parameters)
        {
            return _client.InvokeAsync<TResult>(method, parameters);
        }

        public Task<TResult> InvokeT121Async<TResult>(string method, IReadOnlyDictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            return _client.InvokeAsync<TResult>(method, parameters, cancellationToken);
        }

        public Task<TResult> InvokeT200Async<TResult, TErrorData>(string method)
        {
            return _client.InvokeAsync<TResult, TErrorData>(method);
        }

        public Task<TResult> InvokeT201Async<TResult, TErrorData>(string method, CancellationToken cancellationToken)
        {
            return _client.InvokeAsync<TResult, TErrorData>(method, cancellationToken);
        }

        public Task<TResult> InvokeT210Async<TResult, TErrorData>(string method, IReadOnlyList<object> parameters)
        {
            return _client.InvokeAsync<TResult, TErrorData>(method, parameters);
        }

        public Task<TResult> InvokeT211Async<TResult, TErrorData>(string method, IReadOnlyList<object> parameters, CancellationToken cancellationToken)
        {
            return _client.InvokeAsync<TResult, TErrorData>(method, parameters, cancellationToken);
        }

        public Task<TResult> InvokeT220Async<TResult, TErrorData>(string method, IReadOnlyDictionary<string, object> parameters)
        {
            return _client.InvokeAsync<TResult, TErrorData>(method, parameters);
        }

        public Task<TResult> InvokeT221Async<TResult, TErrorData>(string method, IReadOnlyDictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            return _client.InvokeAsync<TResult, TErrorData>(method, parameters, cancellationToken);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
