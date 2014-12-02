using System;

namespace EqualsVerifier.Util.Exceptions
{
    public class ReflectionException : InternalException
    {
        public ReflectionException(string message) : base(message)
        {
        }

        public ReflectionException(Exception cause) : base("", cause)
        {
        }
    }
}

