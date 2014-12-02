using System;

namespace EqualsVerifier.Util.Exceptions
{
    public class InternalException : Exception
    {
        public InternalException()
        {
        }

        public InternalException(string message) : base(message)
        {
        }

        public InternalException(Exception innerException) : this("", innerException)
        {
        }

        public InternalException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

