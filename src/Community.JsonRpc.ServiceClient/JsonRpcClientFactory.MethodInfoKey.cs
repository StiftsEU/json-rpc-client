// © Alexander Kozlenko. Licensed under the MIT License.

using System.Reflection;

namespace Community.JsonRpc.ServiceClient
{
    public partial class JsonRpcClientFactory
    {
        private readonly struct MethodInfoKey
        {
            private readonly int _hashCode;
            private readonly string _methodName;

            public MethodInfoKey(MethodInfo method)
            {
                _hashCode = CreateHashCode(method);
                _methodName = method.Name;
            }

            private static int CreateHashCode(MethodInfo method)
            {
                var parameters = method.GetParameters();

                unchecked
                {
                    var hashCode = (int)2166136261;

                    hashCode ^= method.Name.GetHashCode();
                    hashCode *= 16777619;

                    for (var i = 0; i < parameters.Length; i++)
                    {
                        hashCode ^= parameters[i].ParameterType.GetHashCode();
                        hashCode *= 16777619;
                    }

                    return hashCode;
                }
            }

            public override int GetHashCode()
            {
                return _hashCode;
            }

            public string MethodName
            {
                get => _methodName;
            }
        }
    }
}