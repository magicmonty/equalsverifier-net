using System;
using Shouldly;
using EqualsVerifier.Util.Exceptions;

namespace EqualsVerifier.TestHelpers
{
    public class IntegrationTestBase
    {
        public void ExpectFailureWithCause<T>(Action action, params string[] fragments) where T: Exception
        {
            Should.Throw<AssertionException>(action);
            try
            {
                action();
            }
            catch (AssertionException e)
            {
                e.InnerException.ShouldNotBe(null);
                e.InnerException.ShouldBeOfType(typeof(T));
                foreach (var fragment in fragments)
                    e.Message.ShouldContain(fragment);
            }
        }

        public void ExpectFailure(Action action, params string[] fragments)
        {
            ExpectException<AssertionException>(action, fragments);
        }

        public void ExpectException<T>(Action action, params string[] fragments) where T: Exception
        {
            Should.Throw<T>(action);
            try
            {
                action();
            }
            catch (Exception e)
            {
                e.ShouldBeOfType(typeof(T));
                foreach (var fragment in fragments)
                    e.Message.ShouldContain(fragment);
            }
        }
    }
}

