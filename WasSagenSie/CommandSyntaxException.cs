using System;
using System.Runtime.Serialization;

namespace WasSagenSie
{
    [Serializable]
    internal class CommandSyntaxException : Exception
    {
        public CommandSyntaxException()
        {
        }

        public CommandSyntaxException(string message) : base(message)
        {
        }

        public CommandSyntaxException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CommandSyntaxException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}