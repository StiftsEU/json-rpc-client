using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace Anemonis.JsonRpc.ServiceClient.Benchmarks.TestSuites
{
    public sealed class JsonRpcClientFactoryBenchmarks
    {
        private static readonly JsonRpcClient _executor = new JsonRpcClient("https://localhost");

        [Benchmark(Description = "Create-PARAMS=U-ERROR=N")]
        public object CreateWithParametersNone()
        {
            return JsonRpcClientFactory.Create<IServiceParametersNone>(_executor);
        }

        [Benchmark(Description = "Create-PARAMS=P-ERROR=N")]
        public object CreateWithParametersByPosition()
        {
            return JsonRpcClientFactory.Create<IServiceParametersByPosition>(_executor);
        }

        [Benchmark(Description = "Create-PARAMS=N-ERROR=N")]
        public object CreateWithParametersByName()
        {
            return JsonRpcClientFactory.Create<IServiceParametersNone>(_executor);
        }

        [Benchmark(Description = "Create-PARAMS=U-ERROR=Y")]
        public object CreateWithErrorDataAndParametersNone()
        {
            return JsonRpcClientFactory.Create<IServiceErrorDataParametersNone>(_executor);
        }

        [Benchmark(Description = "Create-PARAMS=P-ERROR=Y")]
        public object CreateWithErrorDataAndParametersByPosition()
        {
            return JsonRpcClientFactory.Create<IServiceErrorDataParametersByPosition>(_executor);
        }

        [Benchmark(Description = "Create-PARAMS=N-ERROR=Y")]
        public object CreateWithErrorDataAndParametersByName()
        {
            return JsonRpcClientFactory.Create<IServiceErrorDataParametersByName>(_executor);
        }

        #region Types

        public interface IServiceParametersNone
        {
            [JsonRpcMethod("m")]
            Task InvokeAsync();
        }

        public interface IServiceParametersByPosition
        {
            [JsonRpcMethod("m", 0)]
            Task InvokeAsync(long parameter);
        }

        public interface IServiceParametersByName
        {
            [JsonRpcMethod("m", "p")]
            Task InvokeAsync(long parameter);
        }

        public interface IServiceErrorDataParametersNone
        {
            [JsonRpcMethod("m", typeof(long))]
            Task InvokeAsync();
        }

        public interface IServiceErrorDataParametersByPosition
        {
            [JsonRpcMethod("m", typeof(long), 0)]
            Task InvokeAsync(long parameter);
        }

        public interface IServiceErrorDataParametersByName
        {
            [JsonRpcMethod("m", typeof(long), "p")]
            Task InvokeAsync(long parameter);
        }

        #endregion
    }
}