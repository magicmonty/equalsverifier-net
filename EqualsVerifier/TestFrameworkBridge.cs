using System;
using EqualsVerifier.Util;
using EqualsVerifier.Util.Exceptions;

namespace EqualsVerifier
{
    public static class TestFrameworkBridge
    {
        public static void AssertFalse(string message, bool assertion)
        {
            Assert.AssertFalse(message, assertion);
        }

        public static void AssertFalse(ObjectFormatter message, bool assertion)
        {
            Assert.AssertFalse(message, assertion);
        }

        public static void AssertTrue(string message, bool assertion)
        {
            Assert.AssertTrue(message, assertion);
        }

        public static void AssertTrue(ObjectFormatter message, bool assertion)
        {
            Assert.AssertTrue(message, assertion);
        }

        public static void AssertEquals(ObjectFormatter message, object expectedValue, object actualValue)
        {
            Assert.AssertEquals(message, expectedValue, actualValue);
        }

        public static void Fail(ObjectFormatter formatter)
        {
            Assert.Fail(formatter);
        }

        public static void Fail(ObjectFormatter formatter, Exception cause)
        {
            Assert.Fail(formatter, cause);
        }

        public static void AssertionError(string message)
        {
            throw new AssertionException(ObjectFormatter.Of(message));
        }

        public static void AssertionError(string message, Exception cause)
        {
            throw new AssertionException(ObjectFormatter.Of(message), cause);
        }

    }
}

