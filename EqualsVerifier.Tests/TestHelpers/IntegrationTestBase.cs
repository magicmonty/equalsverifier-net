using System;
using Shouldly;
using EqualsVerifier.Util.Exceptions;

namespace EqualsVerifier.TestHelpers
{
    public class IntegrationTestBase
    {
        public void ExpectFailureWithCause<T>(Action action, params string[] fragments) where T: Exception
        {
            ExpectException<T>(action, fragments);
        }

        public void ExpectFailure(Action action, params string[] fragments)
        {
            ExpectException<AssertionException>(action, fragments);
        }

        public void ExpectException<T>(Action action, params string[] fragments) where T: Exception
        {
            Should.Throw<T>(action);
            try {
                action();
            }
            catch (Exception e) {
                e.ShouldBeOfType(typeof(T));
                foreach (var fragment in fragments)
                    e.Message.ShouldContain(fragment);
            }
        }
    }
}

