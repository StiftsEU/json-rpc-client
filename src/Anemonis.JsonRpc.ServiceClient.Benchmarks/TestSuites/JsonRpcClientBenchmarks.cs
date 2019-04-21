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
    public class JsonRpcClientBenchmarks
    {
        private static readonly IReadOnlyDictionary<string, byte[]> _resources = CreateResourceDictionary();
        private static readonly IReadOnlyList<object> _params0 = CreateParametersByPosition();
        private static readonly IReadOnlyDictionary<string, object> _params1 = CreateParametersByName();

        private readonly JsonRpcClient _clientB0I0E0D0 =
            new JsonRpcClient("https://localhost", new HttpClient(new JsonRpcClientBenchmarkHandler()));
        private readonly JsonRpcClient _clientB0I1E0D0 =
            new JsonRpcClient("https://localhost", new HttpClient(new JsonRpcClientBenchmarkHandler(_resources["res_b0i1e0d0"])));
        private readonly JsonRpcClient _clientB0I1E1D0 =
            new JsonRpcClient("https://localhost", new HttpClient(new JsonRpcClientBenchmarkHandler(_resources["res_b0i1e1d0"])));

        private static IReadOnlyDictionary<string, byte[]> CreateResourceDictionary()
        {
            return new Dictionary<string, byte[]>(StringComparer.Ordinal)
            {
                ["res_b0i1e1d0"] = Encoding.UTF8.GetBytes(EmbeddedResourceManager.GetString($"Assets.res_b0i1e1d0.json")),
                ["res_b0i1e0d0"] = Encoding.UTF8.GetBytes(EmbeddedResourceManager.GetString($"Assets.res_b0i1e0d0.json")),
            };
        }

        private static IReadOnlyList<object> CreateParametersByPosition()
        {
            return new object[]
            {
                1L
            };
        }

        private static IReadOnlyDictionary<string, object> CreateParametersByName()
        {
            return new Dictionary<string, object>(2, StringComparer.Ordinal)
            {
                ["p"] = 1L
            };
        }

        [Benchmark(Description = "InvokeAsync-ID=N-PARAMS=U-ERROR=N")]
        public async Task InvokeAsyncB0I0P0E0D0()
        {
            await _clientB0I0E0D0.InvokeAsync("m").ConfigureAwait(false);
        }

        [Benchmark(Description = "InvokeAsync-ID=N-PARAMS=P-ERROR=N")]
        public async Task InvokeAsyncB0I0P1E0D0()
        {
            await _clientB0I0E0D0.InvokeAsync("m", _params0).ConfigureAwait(false);
        }

        [Benchmark(Description = "InvokeAsync-ID=N-PARAMS=N-ERROR=N")]
        public async Task InvokeAsyncB0I0P2E0D0()
        {
            await _clientB0I0E0D0.InvokeAsync("m", _params1).ConfigureAwait(false);
        }

        [Benchmark(Description = "InvokeAsync-ID=Y-PARAMS=U-ERROR=N")]
        public async Task<long> InvokeAsyncB0I1P0E0D0()
        {
            return await _clientB0I1E0D0.InvokeAsync<long>("m", 0L).ConfigureAwait(false);
        }

        [Benchmark(Description = "InvokeAsync-ID=Y-PARAMS=P-ERROR=N")]
        public async Task<long> InvokeAsyncB0I1P1E0D0()
        {
            return await _clientB0I1E0D0.InvokeAsync<long>("m", 0L, _params0).ConfigureAwait(false);
        }

        [Benchmark(Description = "InvokeAsync-ID=Y-PARAMS=N-ERROR=N")]
        public async Task<long> InvokeAsyncB0I1P2E0D0()
        {
            return await _clientB0I1E0D0.InvokeAsync<long>("m", 0L, _params1).ConfigureAwait(false);
        }

        [Benchmark(Description = "InvokeAsync-ID=Y-PARAMS=U-ERROR=Y")]
        public async Task<long> InvokeAsyncB0I1P0E1D0()
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
        public async Task<long> InvokeAsyncB0I1P1E1D0()
        {
            try
            {
                return await _clientB0I1E1D0.InvokeAsync<long>("m", 0L, _params0).ConfigureAwait(false);
            }
            catch (JsonRpcServiceException)
            {
                return default;
            }
        }

        [Benchmark(Description = "InvokeAsync-ID=Y-PARAMS=N-ERROR=Y")]
        public async Task<long> InvokeAsyncB0I1P2E1D0()
        {
            try
            {
                return await _clientB0I1E1D0.InvokeAsync<long>("m", 0L, _params1).ConfigureAwait(false);
            }
            catch (JsonRpcServiceException)
            {
                return default;
            }
        }
    }
}