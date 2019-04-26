// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.Reflection;

namespace Anemonis.JsonRpc.ServiceClient
{
    public partial class JsonRpcClientFactory
    {
        private readonly struct MethodInfoKey
        {
            private readonly string _methodName;
            private readonly ParameterInfo[] _methodParameters;

            public MethodInfoKey(string methodName, ParameterInfo[] methodParameters)
            {
                _methodName = methodName;
                _methodParameters = methodParameters;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = (int)2166136261;

#if NETCOREAPP2_1

                    hashCode ^= _methodName.GetHashCode(StringComparison.Ordinal);

#else

                    hashCode ^= _methodName.GetHashCode();

#endif

                    hashCode *= 16777619;

                    for (var i = 0; i < _methodParameters.Length; i++)
                    {
                        hashCode ^= _methodParameters[i].ParameterType.GetHashCode();
                        hashCode *= 16777619;
                    }

                    return hashCode;
                }
            }

            public string MethodName
            {
                get => _methodName;
            }
        }
    }
}
