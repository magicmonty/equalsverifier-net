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
            TestFrameworkBridge.AssertFalse(message, actualValue);
        }

        protected static void AssertTrue(ObjectFormatter message, bool actualValue)
        {
            TestFrameworkBridge.AssertTrue(message, actualValue);
        }

        protected static void AssertEquals(ObjectFormatter message, object actualValue, object expectedValue)
        {
            TestFrameworkBridge.AssertEquals(message, actualValue, expectedValue);
        }

        protected static void Fail(ObjectFormatter formatter)
        {
            TestFrameworkBridge.Fail(formatter);
        }

        protected static void AssertionError(string message)
        {
            TestFrameworkBridge.AssertionError(message);
        }

        protected static void AssertionError(string message, Exception cause)
        {
            TestFrameworkBridge.AssertionError(message, cause);
        }
    }
}

