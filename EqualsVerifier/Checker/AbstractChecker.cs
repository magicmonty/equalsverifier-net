using NUnit.Framework;
using EqualsVerifier.Util;
using System;

namespace EqualsVerifier.Checker
{
    public abstract class AbstractChecker : IChecker
    {
        public abstract void Check();

        protected static void AssertFalse(ObjectFormatter message, bool actualValue)
        {
            Assert.That(actualValue, Is.False, message.Format());
        }

        protected static void AssertTrue(ObjectFormatter message, bool actualValue)
        {
            Assert.That(actualValue, Is.True, message.Format());
        }

        protected static void AssertEquals(ObjectFormatter message, object actualValue, object expectedValue)
        {
            Assert.That(actualValue, Is.EqualTo(expectedValue), message.Format());
        }

        protected static void Fail(ObjectFormatter formatter)
        {
            Assert.Fail(formatter.Format());
        }

        protected static void AssertionError(string message)
        {
            throw new AssertionException(message);
        }

        protected static void AssertionError(string message, Exception cause)
        {
            throw new AssertionException(message, cause);
        }
    }
}

