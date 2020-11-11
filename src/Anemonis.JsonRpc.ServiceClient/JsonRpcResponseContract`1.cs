// © Alexander Kozlenko. Licensed under the MIT License.

namespace Anemonis.JsonRpc.ServiceClient
{
    internal static class JsonRpcResponseContract<TResult>
    {
        public static readonly JsonRpcResponseContract Instance = new(typeof(TResult));
    }
}
