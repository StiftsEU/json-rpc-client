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
        private readonly IReadOnlyDictionary<string, JsonRpcClient> _clients;
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

            var clients = new Dictionary<string, JsonRpcClient>(assets.Length, StringComparer.Ordinal);

            foreach (var asset in assets)
            {
                var httpContent = EmbeddedResourceManager.GetString($"Assets.{asset}.json");
                var httpClient = new HttpClient(new JsonRpcClientBenchmarkHandler(httpContent));

                clients[asset] = new JsonRpcClient("https://localhost", httpClient);
            }

            _clients = clients;

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
            await _clients["result_false"]
                .InvokeAsync<VoidValue>("m")
                .ConfigureAwait(false);
        }

        [Benchmark(Description = "not-pos")]
        public async Task InvokeMethodWithVoidResultAndParametersByPosition()
        {
            await _clients["result_false"]
                .InvokeAsync<VoidValue>("m", _parametersByPosition)
                .ConfigureAwait(false);
        }

        [Benchmark(Description = "not-nam")]
        public async Task InvokeMethodWithVoidResultAndParametersByName()
        {
            await _clients["result_false"]
                .InvokeAsync<VoidValue>("m", _parametersByName)
                .ConfigureAwait(false);
        }

        [Benchmark(Description = "err-non")]
        public async Task InvokeMethodWithErrorResultAndNoParameters()
        {
            try
            {
                await _clients["result_true_success_false"]
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
                await _clients["result_true_success_false"]
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
                await _clients["result_true_success_false"]
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
            await _clients["result_true_success_true"]
                .InvokeAsync<long>("m")
                .ConfigureAwait(false);
        }

        [Benchmark(Description = "scs-pos")]
        public async Task InvokeMethodWithValueResultAndParametersByPosition()
        {
            await _clients["result_true_success_true"]
                .InvokeAsync<long>("m", _parametersByPosition)
                .ConfigureAwait(false);
        }

        [Benchmark(Description = "scs-nam")]
        public async Task InvokeMethodWithValueResultAndParametersByName()
        {
            await _clients["result_true_success_true"]
                .InvokeAsync<long>("m", _parametersByName)
                .ConfigureAwait(false);
        }
    }
}