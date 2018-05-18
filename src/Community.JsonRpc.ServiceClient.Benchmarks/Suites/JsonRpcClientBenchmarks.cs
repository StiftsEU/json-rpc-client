using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Community.JsonRpc.ServiceClient.Benchmarks.Internal;
using Community.JsonRpc.ServiceClient.Benchmarks.Resources;

namespace Community.JsonRpc.ServiceClient.Benchmarks.Suites
{
    public abstract class JsonRpcClientBenchmarks
    {
        private static readonly IReadOnlyDictionary<string, string> _resources = CreateResourceDictionary();
        private static readonly IReadOnlyList<object> _parametersByPosition = CreateParametersByPosition();
        private static readonly IReadOnlyDictionary<string, object> _parametersByName = CreateParametersByName();

        private readonly JsonRpcClient _client;

        protected JsonRpcClientBenchmarks()
        {
            var contents = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["not-non"] = null,
                ["not-pos"] = null,
                ["not-nam"] = null,
                ["err-non"] = _resources["success_false"],
                ["err-pos"] = _resources["success_false"],
                ["err-nam"] = _resources["success_false"],
                ["scs-non"] = _resources["success_true"],
                ["scs-pos"] = _resources["success_true"],
                ["scs-nam"] = _resources["success_true"]
            };

            _client = new JsonRpcClient("https://localhost", new HttpClient(new JsonRpcClientBenchmarkHandler(contents)));
        }

        private static IReadOnlyDictionary<string, string> CreateResourceDictionary()
        {
            var resources = new Dictionary<string, string>(StringComparer.Ordinal);

            foreach (var code in GetResponseCodes())
            {
                resources[code] = EmbeddedResourceManager.GetString($"Assets.{code}.json");
            }

            return resources;
        }

        private static IReadOnlyList<object> CreateParametersByPosition()
        {
            return new object[]
            {
                1L,
                2L
            };
        }

        private static IReadOnlyDictionary<string, object> CreateParametersByName()
        {
            return new Dictionary<string, object>(2, StringComparer.Ordinal)
            {
                ["p1"] = 1L,
                ["p2"] = 2L
            };
        }

        private static IEnumerable<string> GetResponseCodes()
        {
            return new[] { "success_false", "success_true" };
        }

        [Benchmark(Description = "not-non")]
        public async Task<VoidValue> InvokeMethodWithVoidResultAndNoParameters()
        {
            return await _client.InvokeAsync<VoidValue>("not-non");
        }

        [Benchmark(Description = "not-pos")]
        public async Task<VoidValue> InvokeMethodWithVoidResultAndParametersByPosition()
        {
            return await _client.InvokeAsync<VoidValue>("not-pos", _parametersByPosition);
        }

        [Benchmark(Description = "not-nam")]
        public async Task<VoidValue> InvokeMethodWithVoidResultAndParametersByName()
        {
            return await _client.InvokeAsync<VoidValue>("not-nam", _parametersByName);
        }

        [Benchmark(Description = "err-non")]
        public async Task<long> InvokeMethodWithErrorResultAndNoParameters()
        {
            try
            {
                return await _client.InvokeAsync<long>("err-non");
            }
            catch (JsonRpcServiceException)
            {
                return default;
            }
        }

        [Benchmark(Description = "err-pos")]
        public async Task<long> InvokeMethodWithErrorResultAndParametersByPosition()
        {
            try
            {
                return await _client.InvokeAsync<long>("err-pos", _parametersByPosition);
            }
            catch (JsonRpcServiceException)
            {
                return default;
            }
        }

        [Benchmark(Description = "err-nam")]
        public async Task<long> InvokeMethodWithErrorResultAndParametersByName()
        {
            try
            {
                return await _client.InvokeAsync<long>("err-nam", _parametersByName);
            }
            catch (JsonRpcServiceException)
            {
                return default;
            }
        }

        [Benchmark(Description = "scs-non")]
        public async Task<long> InvokeMethodWithValueResultAndNoParameters()
        {
            return await _client.InvokeAsync<long>("scs-non");
        }

        [Benchmark(Description = "scs-pos")]
        public async Task<long> InvokeMethodWithValueResultAndParametersByPosition()
        {
            return await _client.InvokeAsync<long>("scs-pos", _parametersByPosition);
        }

        [Benchmark(Description = "scs-nam")]
        public async Task<long> InvokeMethodWithValueResultAndParametersByName()
        {
            return await _client.InvokeAsync<long>("scs-nam", _parametersByName);
        }
    }
}