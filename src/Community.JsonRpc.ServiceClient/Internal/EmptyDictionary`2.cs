using System.Collections.Generic;

namespace Community.JsonRpc.ServiceClient.Internal
{
    internal static class EmptyDictionary<TKey, TValue>
    {
        public static readonly Dictionary<TKey, TValue> Instance = new Dictionary<TKey, TValue>(0);
    }
}