using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Anemonis.JsonRpc.ServiceClient.Benchmarks.Resources;
using Anemonis.JsonRpc.ServiceClient.Benchmarks.TestStubs;
using BenchmarkDotNet.Attributes;

namespace Anemonis.JsonRpc.ServiceClient.Benchmarks.TestSuites
{
    public sealed class JsonRpcClientBenchmarks
    {
        private static readonly IReadOnlyDictionary<string, byte[]> _resources = CreateResourceDictionary();
        private static readonly IReadOnlyList<object> params0 = CreateParametersByPosition();
        private static readonly IReadOnlyDictionary<string, object> params1 = CreateParametersByName();

        private readonly JsonRpcClient _clientB0I0E0D0 =
            new JsonRpcClient("https://localhost", new HttpClient(new JsonRpcClientBenchmarkHandler()));
        private readonly JsonRpcClient _clientB0I1E0D0 =
            new JsonRpcClient("https://localhost", new HttpClient(new JsonRpcClientBenchmarkHandler(_resources["res_b0i1e0d0"])));
        private readonly JsonRpcClient _clientB0I1E1D0 =
            new JsonRpcClient("https://localhost", new HttpClient(new JsonRpcClientBenchmarkHandler(_resources["res_b0i1e1d0"])));

        private static IReadOnlyDictionary<string, byte[]> CreateResourceDictionary()
        {
            var resources = new Dictionary<string, byte[]>(StringComparer.Ordinal);

            foreach (var code in GetResponseCodes())
            {
                resources[code] = Encoding.UTF8.GetBytes(EmbeddedResourceManager.GetString($"Assets.{code}.json"));
            }

            return resources;
        }

        private static IReadOnlyList<object> CreateParametersByPosition()
        {
            return new object[] { 1L, 2L };
        }

        private static IReadOnlyDictionary<string, object> CreateParametersByName()
        {
            return new Dictionary<string, object>(2, StringComparer.Ordinal) { ["p1"] = 1L, ["p2"] = 2L };
        }

        private static IEnumerable<string> GetResponseCodes()
        {
            return new[]
            {
                "res_b0i1e1d0",
                "res_b0i1e0d0"
            };
        }

        [Benchmark(Description = "InvokeAsync-ID=N-PARAMS=U-ERROR=N")]
        public async Task InvokeAsyncWithNotificationAndNoParams()
        {
            await _clientB0I0E0D0.InvokeAsync("m").ConfigureAwait(false);
        }

        [Benchmark(Description = "InvokeAsync-ID=N-PARAMS=P-ERROR=N")]
        public async Task InvokeAsyncWithNotificationAndParamsByPosition()
        {
            await _clientB0I0E0D0.InvokeAsync("m", params0).ConfigureAwait(false);
        }

        [Benchmark(Description = "InvokeAsync-ID=N-PARAMS=N-ERROR=N")]
        public async Task InvokeAsyncWithNotificationAndParamsByName()
        {
            await _clientB0I0E0D0.InvokeAsync("m", params1).ConfigureAwait(false);
        }

        [Benchmark(Description = "InvokeAsync-ID=Y-PARAMS=U-ERROR=N")]
        public async Task<long> InvokeAsyncWithResponseResultAndNoParams()
        {
            return await _clientB0I1E0D0.InvokeAsync<long>("m", 0L).ConfigureAwait(false);
        }

        [Benchmark(Description = "InvokeAsync-ID=Y-PARAMS=P-ERROR=N")]
        public async Task<long> InvokeAsyncWithResponseResultAndParamsByPosition()
        {
            return await _clientB0I1E0D0.InvokeAsync<long>("m", 0L, params0).ConfigureAwait(false);
        }

        [Benchmark(Description = "InvokeAsync-ID=Y-PARAMS=N-ERROR=N")]
        public async Task<long> InvokeAsyncWithResponseResultAndParamsByName()
        {
            return await _clientB0I1E0D0.InvokeAsync<long>("m", 0L, params1).ConfigureAwait(false);
        }

        [Benchmark(Description = "InvokeAsync-ID=Y-PARAMS=U-ERROR=Y")]
        public async Task<long> InvokeAsyncWithResponseErrorAndNoParams()
        {
            try
            {
                return await _clientB0I1E1D0.InvokeAsync<long>("m", 0L).ConfigureAwait(false);
            }
            catch (JsonRpcServiceException)
            {
                return default;
            }
        }

        [Benchmark(Description = "InvokeAsync-ID=Y-PARAMS=P-ERROR=Y")]
        public async Task<long> InvokeAsyncWithResponseErrorAndParamsByPosition()
        {
            try
            {
                return await _clientB0I1E1D0.InvokeAsync<long>("m", 0L, params0).ConfigureAwait(false);
            }
            catch (JsonRpcServiceException)
            {
                return default;
            }
        }

        [Benchmark(Description = "InvokeAsync-ID=Y-PARAMS=N-ERROR=Y")]
        public async Task<long> InvokeAsyncWithResponseErrorAndParamsByName()
        {
            try
            {
                return await _clientB0I1E1D0.InvokeAsync<long>("m", 0L, params1).ConfigureAwait(false);
            }
            catch (JsonRpcServiceException)
            {
                return default;
            }
        }
    }
}