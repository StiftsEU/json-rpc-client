using Anemonis.JsonRpc.ServiceClient.Benchmarks.TestStubs;

using BenchmarkDotNet.Attributes;

namespace Anemonis.JsonRpc.ServiceClient.Benchmarks.TestSuites
{
    public class JsonRpcClientFactoryBenchmarks
    {
        private static readonly JsonRpcClient _executor = new JsonRpcClient("https://localhost");

        [Benchmark(Description = "Create-PARAMS=U-ERROR=N")]
        public object CreateP0E0D0()
        {
            return JsonRpcClientFactory.Create<IJsonRpcServiceP0E0D0>(_executor);
        }

        [Benchmark(Description = "Create-PARAMS=P-ERROR=N")]
        public object CreateP1E0D0()
        {
            return JsonRpcClientFactory.Create<IJsonRpcServiceP1E0D0>(_executor);
        }

        [Benchmark(Description = "Create-PARAMS=N-ERROR=N")]
        public object CreateP2E0D0()
        {
            return JsonRpcClientFactory.Create<IJsonRpcServiceP0E0D0>(_executor);
        }

        [Benchmark(Description = "Create-PARAMS=U-ERROR=Y")]
        public object CreateP0E1D1()
        {
            return JsonRpcClientFactory.Create<IJsonRpcServiceP0E1D1>(_executor);
        }

        [Benchmark(Description = "Create-PARAMS=P-ERROR=Y")]
        public object CreateP1E1D1()
        {
            return JsonRpcClientFactory.Create<IJsonRpcServiceP1E1D1>(_executor);
        }

        [Benchmark(Description = "Create-PARAMS=N-ERROR=Y")]
        public object CreateP2E1D1()
        {
            return JsonRpcClientFactory.Create<IJsonRpcServiceP2E1D1>(_executor);
        }
    }
}
