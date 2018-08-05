// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.Data.JsonRpc;
using Community.JsonRpc.ServiceClient.Resources;

namespace Community.JsonRpc.ServiceClient
{
    /// <summary>Defines a JSON-RPC method contract.</summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class JsonRpcMethodAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="JsonRpcMethodAttribute" /> class.</summary>
        /// <param name="methodName">The name of a JSON-RPC method.</param>
        /// <exception cref="ArgumentException"><paramref name="methodName" /> is a system extension method.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="methodName" /> is <see langword="null" />.</exception>
        public JsonRpcMethodAttribute(string methodName)
        {
            if (methodName == null)
            {
                throw new ArgumentNullException(nameof(methodName));
            }
            if (JsonRpcSerializer.IsSystemMethod(methodName))
            {
                throw new ArgumentException(Strings.GetString("invoke.method.invalid_name"), nameof(methodName));
            }

            MethodName = methodName;
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcMethodAttribute" /> class.</summary>
        /// <param name="methodName">The name of a JSON-RPC method.</param>
        /// <param name="parameterPositions">The corresponding positions of the JSON-RPC method parameters for the interface method parameters.</param>
        /// <exception cref="ArgumentException"><paramref name="methodName" /> is a system extension method.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="methodName" /> or <paramref name="parameterPositions" /> is <see langword="null" />.</exception>
        public JsonRpcMethodAttribute(string methodName, params int[] parameterPositions)
            : this(methodName)
        {
            if (parameterPositions == null)
            {
                throw new ArgumentNullException(nameof(parameterPositions));
            }

            ParameterPositions = parameterPositions;
            ParametersType = JsonRpcParametersType.ByPosition;
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcMethodAttribute" /> class.</summary>
        /// <param name="methodName">The name of a JSON-RPC method.</param>
        /// <param name="parameterNames">The corresponding names of the JSON-RPC method parameters for the interface method parameters.</param>
        /// <exception cref="ArgumentException"><paramref name="methodName" /> is a system extension method.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="methodName" /> or <paramref name="parameterNames" /> is <see langword="null" />.</exception>
        public JsonRpcMethodAttribute(string methodName, params string[] parameterNames)
            : this(methodName)
        {
            if (parameterNames == null)
            {
                throw new ArgumentNullException(nameof(parameterNames));
            }

            ParameterNames = parameterNames;
            ParametersType = JsonRpcParametersType.ByName;
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcMethodAttribute" /> class.</summary>
        /// <param name="methodName">The name of a JSON-RPC method.</param>
        /// <param name="errorDataType">The type of JSON-RPC method error data.</param>
        /// <exception cref="ArgumentException"><paramref name="methodName" /> is a system extension method.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="methodName" /> or <paramref name="errorDataType" /> is <see langword="null" />.</exception>
        public JsonRpcMethodAttribute(string methodName, Type errorDataType)
            : this(methodName)
        {
            if (errorDataType == null)
            {
                throw new ArgumentNullException(nameof(errorDataType));
            }

            ErrorDataType = errorDataType;
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcMethodAttribute" /> class.</summary>
        /// <param name="methodName">The name of a JSON-RPC method.</param>
        /// <param name="errorDataType">The type of JSON-RPC method error data.</param>
        /// <param name="parameterPositions">The corresponding positions of the JSON-RPC method parameters for the interface method parameters.</param>
        /// <exception cref="ArgumentException"><paramref name="methodName" /> is a system extension method.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="methodName" />, <paramref name="methodName" />, or <paramref name="parameterPositions" /> is <see langword="null" />.</exception>
        public JsonRpcMethodAttribute(string methodName, Type errorDataType, params int[] parameterPositions)
            : this(methodName, errorDataType)
        {
            if (parameterPositions == null)
            {
                throw new ArgumentNullException(nameof(parameterPositions));
            }

            ParameterPositions = parameterPositions;
            ParametersType = JsonRpcParametersType.ByPosition;
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcMethodAttribute" /> class.</summary>
        /// <param name="methodName">The name of a JSON-RPC method.</param>
        /// <param name="errorDataType">The type of JSON-RPC method error data.</param>
        /// <param name="parameterNames">The corresponding names of the JSON-RPC method parameters for the interface method parameters.</param>
        /// <exception cref="ArgumentException"><paramref name="methodName" /> is a system extension method.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="methodName" />, <paramref name="methodName" />, or <paramref name="parameterNames" /> is <see langword="null" />.</exception>
        public JsonRpcMethodAttribute(string methodName, Type errorDataType, params string[] parameterNames)
            : this(methodName, errorDataType)
        {
            if (parameterNames == null)
            {
                throw new ArgumentNullException(nameof(parameterNames));
            }

            ParameterNames = parameterNames;
            ParametersType = JsonRpcParametersType.ByName;
        }

        internal string MethodName
        {
            get;
        }

        internal JsonRpcParametersType ParametersType
        {
            get;
        }

        internal Type ErrorDataType
        {
            get;
        }

        internal int[] ParameterPositions
        {
            get;
        }

        internal string[] ParameterNames
        {
            get;
        }
    }
}