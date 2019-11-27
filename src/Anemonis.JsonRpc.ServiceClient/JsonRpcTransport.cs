// © Alexander Kozlenko. Licensed under the MIT License.

using System.Text;

namespace Anemonis.JsonRpc.ServiceClient
{
    internal static class JsonRpcTransport
    {
        public const string MediaType = "application/json";

        public static readonly string Charset = Encoding.UTF8.WebName;
    }
}
