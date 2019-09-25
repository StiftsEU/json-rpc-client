// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.Reflection;

namespace Anemonis.JsonRpc.ServiceClient
{
    public partial class JsonRpcClientFactory
    {
        private readonly struct MethodInfoKey : IEquatable<MethodInfoKey>
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
                var hashCode = new HashCode();

                hashCode.Add(_methodName);

                for (var i = 0; i < _methodParameters.Length; i++)
                {
                    hashCode.Add(_methodParameters[i].ParameterType);
                }

                return hashCode.ToHashCode();
            }

            public override bool Equals(object obj)
            {
                return (obj is MethodInfoKey other) && Equals(other);
            }

            public bool Equals(MethodInfoKey other)
            {
                if (other._methodName != _methodName)
                {
                    return false;
                }

                for (var i = 0; i < _methodParameters.Length; i++)
                {
                    if (!other._methodParameters[i].ParameterType.Equals(_methodParameters[i].ParameterType))
                    {
                        return false;
                    }
                }

                return true;
            }

            public string MethodName
            {
                get => _methodName;
            }
        }
    }
}
