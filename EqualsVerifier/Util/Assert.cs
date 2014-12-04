using EqualsVerifier.Util.Exceptions;
using System;

namespace EqualsVerifier.Util
{
    public static class Assert
    {
        public static void AssertEquals(ObjectFormatter message, object expected, object actual)
        {
            if (!expected.Equals(actual)) {
                throw new AssertionException(message);
            }
        }

        public static void AssertFalse(string message, bool assertion)
        {
            AssertFalse(ObjectFormatter.Of(message), assertion);
        }

        public static void AssertFalse(ObjectFormatter message, bool assertion)
        {
            if (assertion) {
                throw new AssertionException(message);
            }
        }

        public static void AssertTrue(string message, bool assertion)
        {
            AssertTrue(ObjectFormatter.Of(message), assertion);
        }

        public static void AssertTrue(ObjectFormatter message, bool assertion)
        {
            if (!assertion) {
                throw new AssertionException(message);
            }
        }

        public static void Fail(ObjectFormatter message)
        {
            throw new AssertionException(message);
        }

        public static void Fail(ObjectFormatter message, Exception cause)
        {
            throw new AssertionException(message, cause);
        }
    }
}

