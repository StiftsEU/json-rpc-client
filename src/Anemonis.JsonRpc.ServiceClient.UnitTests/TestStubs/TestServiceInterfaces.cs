using System;
using System.Threading;
using System.Threading.Tasks;

namespace Anemonis.JsonRpc.ServiceClient.UnitTests.TestStubs
{
    public interface IServiceInvalidReturnType
    {
        [JsonRpcMethod("m")]
        long InvokeAsync();
    }

    public interface IServiceParameterRef
    {
        [JsonRpcMethod("m")]
        Task InvokeAsync(ref long parameter);
    }

    public interface IServiceParameterOut
    {
        [JsonRpcMethod("m")]
        Task InvokeAsync(out long parameter);
    }

    public interface IServiceParametersNoneCountInvalid
    {
        [JsonRpcMethod("m")]
        Task InvokeAsync(long parameter);
    }

    public interface IServiceParametersByPositionCountInvalid
    {
        [JsonRpcMethod("m", 0)]
        Task InvokeAsync(long parameter1, long paramerer2);
    }

    public interface IServiceParametersByNameCountInvalid
    {
        [JsonRpcMethod("m", "p")]
        Task InvokeAsync(long parameter1, long paramerer2);
    }

    public interface IServiceParametersByPositionPositionsInvalid
    {
        [JsonRpcMethod("m", 0, 0)]
        Task InvokeAsync(long parameter1, long paramerer2);
    }

    public interface IServiceParametersByNameNamesInvalid
    {
        [JsonRpcMethod("m", "p", "p")]
        Task InvokeAsync(long parameter1, long paramerer2);
    }

    public interface IServiceProperty
    {
        long Status
        {
            get;
        }
    }

    public interface IServiceEvent
    {
        event EventHandler<EventArgs> Completed;
    }

    public interface IServiceMethodWithBody
    {
        [JsonRpcMethod("m0")]
        Task<int> InvokeAsync()
        {
            return Task.FromResult(42);
        }
    }

    public interface IServiceSharedMethodName
    {
        [JsonRpcMethod("m0")]
        Task InvokeAsync();

        [JsonRpcMethod("m1", 0)]
        Task InvokeAsync(long parameter);

        [JsonRpcMethod("m2", 0)]
        Task InvokeAsync(string parameter);
    }

    public interface IService : IDisposable
    {
        [JsonRpcMethod("m000")]
        Task InvokeT000Async();

        [JsonRpcMethod("m001")]
        Task InvokeT001Async(CancellationToken cancellationToken);

        [JsonRpcMethod("m010", 0)]
        Task InvokeT010Async(long parameter);

        [JsonRpcMethod("m011", 0)]
        Task InvokeT011Async(long parameter, CancellationToken cancellationToken);

        [JsonRpcMethod("m020", "p")]
        Task InvokeT020Async(long parameter);

        [JsonRpcMethod("m021", "p")]
        Task InvokeT021Async(long parameter, CancellationToken cancellationToken);

        [JsonRpcMethod("m100")]
        Task<long> InvokeT100Async();

        [JsonRpcMethod("m101")]
        Task<long> InvokeT101Async(CancellationToken cancellationToken);

        [JsonRpcMethod("m110", 0)]
        Task<long> InvokeT110Async(long parameter);

        [JsonRpcMethod("m111", 0)]
        Task<long> InvokeT111Async(long parameter, CancellationToken cancellationToken);

        [JsonRpcMethod("m120", "p")]
        Task<long> InvokeT120Async(long parameter);

        [JsonRpcMethod("m121", "p")]
        Task<long> InvokeT121Async(long parameter, CancellationToken cancellationToken);

        [JsonRpcMethod("m200", typeof(long))]
        Task<long> InvokeT200Async();

        [JsonRpcMethod("m201", typeof(long))]
        Task<long> InvokeT201Async(CancellationToken cancellationToken);

        [JsonRpcMethod("m210", typeof(long), 0)]
        Task<long> InvokeT210Async(long parameter);

        [JsonRpcMethod("m211", typeof(long), 0)]
        Task<long> InvokeT211Async(long parameter, CancellationToken cancellationToken);

        [JsonRpcMethod("m220", typeof(long), "p")]
        Task<long> InvokeT220Async(long parameter);

        [JsonRpcMethod("m221", typeof(long), "p")]
        Task<long> InvokeT221Async(long parameter, CancellationToken cancellationToken);
    }
}
