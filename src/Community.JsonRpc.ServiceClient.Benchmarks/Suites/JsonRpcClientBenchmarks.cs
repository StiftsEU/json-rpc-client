using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Community.JsonRpc.ServiceClient.Benchmarks.Framework;
using Community.JsonRpc.ServiceClient.Benchmarks.Internal;
using Community.JsonRpc.ServiceClient.Benchmarks.Resources;

namespace Community.JsonRpc.ServiceClient.Benchmarks.Suites
{
    [BenchmarkSuite(nameof(JsonRpcClient))]
    public abstract class JsonRpcClientBenchmarks
    {
        private static readonly Uri _serviceUrl = new Uri("https://localhost", UriKind.Absolute);

        private readonly IReadOnlyDictionary<string, HttpMessageInvoker> _invokers;
        private readonly IReadOnlyList<object> _parametersByPosition;
        private readonly IReadOnlyDictionary<string, object> _parametersByName;

        protected JsonRpcClientBenchmarks()
        {
            var assets = new[]
            {
                "result_false",
                "result_true_success_false",
                "result_true_success_true"
            };

            var clients = new Dictionary<string, HttpMessageInvoker>(assets.Length, StringComparer.Ordinal);

            foreach (var asset in assets)
            {
                clients[asset] = new HttpClient(new JsonRpcClientBenchmarkHandler(EmbeddedResourceManager.GetString($"Assets.{asset}.json")));
            }

            _invokers = clients;

            _parametersByPosition = new object[]
            {
                1L,
                2L
            };
            _parametersByName = new Dictionary<string, object>(2, StringComparer.Ordinal)
            {
                ["p1"] = 1L,
                ["p2"] = 2L
            };
        }

        [Benchmark(Description = "not-non")]
        public async Task InvokeMethodWithVoidResultAndNoParameters()
        {
            await new JsonRpcClient(_serviceUrl, _invokers["result_false"])
                .InvokeAsync<VoidValue>("m")
                .ConfigureAwait(false);
        }

        [Benchmark(Description = "not-pos")]
        public async Task InvokeMethodWithVoidResultAndParametersByPosition()
        {
            await new JsonRpcClient(_serviceUrl, _invokers["result_false"])
                .InvokeAsync<VoidValue>("m", _parametersByPosition)
                .ConfigureAwait(false);
        }

        [Benchmark(Description = "not-nam")]
        public async Task InvokeMethodWithVoidResultAndParametersByName()
        {
            await new JsonRpcClient(_serviceUrl, _invokers["result_false"])
                .InvokeAsync<VoidValue>("m", _parametersByName)
                .ConfigureAwait(false);
        }

        [Benchmark(Description = "err-non")]
        public async Task InvokeMethodWithErrorResultAndNoParameters()
        {
            try
            {
                await new JsonRpcClient(_serviceUrl, _invokers["result_true_success_false"])
                    .InvokeAsync<long>("m")
                    .ConfigureAwait(false);
            }
            catch (JsonRpcServiceException)
            {
            }
        }

        [Benchmark(Description = "err-pos")]
        public async Task InvokeMethodWithErrorResultAndParametersByPosition()
        {
            try
            {
                await new JsonRpcClient(_serviceUrl, _invokers["result_true_success_false"])
                    .InvokeAsync<long>("m", _parametersByPosition)
                    .ConfigureAwait(false);
            }
            catch (JsonRpcServiceException)
            {
            }
        }

        [Benchmark(Description = "err-nam")]
        public async Task InvokeMethodWithErrorResultAndParametersByName()
        {
            try
            {
                await new JsonRpcClient(_serviceUrl, _invokers["result_true_success_false"])
                    .InvokeAsync<long>("m", _parametersByName)
                    .ConfigureAwait(false);
            }
            catch (JsonRpcServiceException)
            {
            }
        }

        [Benchmark(Description = "scs-non")]
        public async Task InvokeMethodWithValueResultAndNoParameters()
        {
            await new JsonRpcClient(_serviceUrl, _invokers["result_true_success_true"])
                .InvokeAsync<long>("m")
                .ConfigureAwait(false);
        }

        [Benchmark(Description = "scs-pos")]
        public async Task InvokeMethodWithValueResultAndParametersByPosition()
        {
            await new JsonRpcClient(_serviceUrl, _invokers["result_true_success_true"])
                .InvokeAsync<long>("m", _parametersByPosition)
                .ConfigureAwait(false);
        }

        [Benchmark(Description = "scs-nam")]
        public async Task InvokeMethodWithValueResultAndParametersByName()
        {
            await new JsonRpcClient(_serviceUrl, _invokers["result_true_success_true"])
                .InvokeAsync<long>("m", _parametersByName)
                .ConfigureAwait(false);
        }
    }
}