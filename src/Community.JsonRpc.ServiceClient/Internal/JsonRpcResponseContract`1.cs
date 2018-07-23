// © Alexander Kozlenko. Licensed under the MIT License.

using System.Data.JsonRpc;

namespace Community.JsonRpc.ServiceClient.Internal
{
    internal static class JsonRpcResponseContract<T>
    {
        public static readonly JsonRpcResponseContract Instance = new JsonRpcResponseContract(typeof(T));
    }
}