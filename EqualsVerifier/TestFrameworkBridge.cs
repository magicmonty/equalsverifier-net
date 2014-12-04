using System;
using NUnit.Framework;
using EqualsVerifier.Util;

namespace EqualsVerifier
{
    public static class TestFrameworkBridge
    {
        public static void AssertFalse(ObjectFormatter message, bool actualValue)
        {
            Assert.That(actualValue, Is.False, message.Format());
        }

        public static void AssertTrue(ObjectFormatter message, bool actualValue)
        {
            Assert.That(actualValue, Is.True, message.Format());
        }

        public static void AssertEquals(ObjectFormatter message, object actualValue, object expectedValue)
        {
            Assert.That(actualValue, Is.EqualTo(expectedValue), message.Format());
        }

        public static void Fail(ObjectFormatter formatter)
        {
            Assert.Fail(formatter.Format());
        }

        public static void AssertionError(string message)
        {
            throw new AssertionException(message);
        }

        public static void AssertionError(string message, Exception cause)
        {
            throw new AssertionException(message, cause);
        }

    }
}

