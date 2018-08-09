using System;

namespace OverrideCustomPage.Exceptions
{
    public class OverrideCustomPageException : Exception
    {
        public OverrideCustomPageException()
        {
        }

        public OverrideCustomPageException(string message)
            : base(message)
        {
        }

        public OverrideCustomPageException(string message, Exception inner)
            : base(message, inner)
        {
        }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client.
        protected OverrideCustomPageException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
        }
    }
}