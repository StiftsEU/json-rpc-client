using System;

namespace Community.JsonRpc.ServiceClient
{
    /// <summary>Represents an error that occur during service method invocation.</summary>
    public sealed class JsonRpcServiceException : Exception
    {
        internal JsonRpcServiceException(long code, string message)
            : base(message)
        {
            Code = code;
        }

        /// <summary>Gets a number that indicates the error type that occurred.</summary>
        public long Code
        {
            get;
        }
    }
}