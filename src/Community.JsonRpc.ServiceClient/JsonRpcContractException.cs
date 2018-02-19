using System;

namespace Community.JsonRpc.ServiceClient
{
    /// <summary>Represents an error that occur during service result handling.</summary>
    public sealed class JsonRpcContractException : Exception
    {
        internal JsonRpcContractException(string message)
            : base(message)
        {
        }

        internal JsonRpcContractException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}