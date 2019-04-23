using System.Threading.Tasks;

namespace Anemonis.JsonRpc.ServiceClient.Benchmarks.TestStubs
{
    public interface IJsonRpcServiceP0E0D0
    {
        [JsonRpcMethod("m")]
        Task InvokeAsync();
    }

    public interface IJsonRpcServiceP1E0D0
    {
        [JsonRpcMethod("m", 0)]
        Task InvokeAsync(long parameter);
    }

    public interface IJsonRpcServiceP2E0D0
    {
        [JsonRpcMethod("m", "p")]
        Task InvokeAsync(long parameter);
    }

    public interface IJsonRpcServiceP0E1D1
    {
        [JsonRpcMethod("m", typeof(long))]
        Task InvokeAsync();
    }

    public interface IJsonRpcServiceP1E1D1
    {
        [JsonRpcMethod("m", typeof(long), 0)]
        Task InvokeAsync(long parameter);
    }

    public interface IJsonRpcServiceP2E1D1
    {
        [JsonRpcMethod("m", typeof(long), "p")]
        Task InvokeAsync(long parameter);
    }
}
