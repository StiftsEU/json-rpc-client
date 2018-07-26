// © Alexander Kozlenko. Licensed under the MIT License.

using System.Data.JsonRpc;

namespace Community.JsonRpc.ServiceClient
{
    internal static class JsonRpcResponseContract<TResult>
    {
        public static readonly JsonRpcResponseContract Instance = new JsonRpcResponseContract(typeof(TResult));
    }
}