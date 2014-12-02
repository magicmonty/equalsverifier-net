using System;

namespace EqualsVerifier.Util.Exceptions
{
    public class AssertionException : InternalException
    {
        public AssertionException(ObjectFormatter message) : base(message.Format())
        {
        }

        public AssertionException(ObjectFormatter message, Exception internalException) : base(message.Format(), internalException)
        {
        }
    }
}

