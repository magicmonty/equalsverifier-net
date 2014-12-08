using System;
using EqualsVerifier.TestHelpers;
using NUnit.Framework;

namespace EqualsVerifier.Integration.ExtendedContract
{
    [TestFixture]
    public class SignatureTest : IntegrationTestBase
    {
        const string OVERLOADED = "Overloaded";
        const string SIGNATURE_SHOULD_BE = "Signature should be";
        const string SIGNATURE = "public boolean equals(Object obj)";

        #warning TODO Check the use of IEquatable<T> here!

        [Test]
        public void WhenEqualsIsOverloadedWithTypeInsteadOfObject_ThenFail()
        {
            ExpectOverloadFailure(
                () => EqualsVerifier.ForType<OverloadedWithOwnType>().Verify(),
                "Parameter should be an Object, not " + typeof(OverloadedWithOwnType).Name);
        }

        [Test]
        public void WhenEqualsIsOverloadedWithTwoParameters_ThenFail()
        {
            ExpectOverloadFailure(
                () => EqualsVerifier.ForType<OverloadedWithTwoParameters>().Verify(),
                "Too many parameters");
        }

        [Test]
        public void WhenEqualsIsOverloadedWithNoParameter_ThenFail()
        {
            ExpectOverloadFailure(
                () => EqualsVerifier.ForType<OverloadedWithNoParameter>().Verify(),
                "No parameter");
        }

        [Test]
        public void WhenEqualsIsOverloadedWithUnrelatedParameter_ThenFail()
        {
            ExpectOverloadFailure(
                () => EqualsVerifier.ForType<OverloadedWithUnrelatedParameter>().Verify(),
                "Parameter should be of type 'object'");
        }

        [Test]
        public void WhenEqualsIsProperlyOverriddenButAlsoOverloaded_ThenFail()
        {
            ExpectOverloadFailure(
                () => EqualsVerifier.ForType<OverloadedAndOverridden>().Verify(),
                "More than one equals method found");
        }

        [Test]
        public void WhenEqualsIsNeitherOverriddenOrOverloaded_ThenSucceed()
        {
            EqualsVerifier
                .ForType<NoEqualsMethod>()
                .Verify();
        }

        void ExpectOverloadFailure(Action action, string extraMessage)
        {
            ExpectFailure(
                action,
                OVERLOADED, 
                SIGNATURE_SHOULD_BE, 
                SIGNATURE, extraMessage);
        }

        #pragma warning disable 108
        #pragma warning disable 114
        #pragma warning disable 414
        public sealed class OverloadedWithOwnType
        {
            readonly int _i;

            OverloadedWithOwnType(int i)
            {
                _i = i;
            }

            public bool Equals(OverloadedWithOwnType obj)
            {
                return obj != null && _i == obj._i;
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        public sealed class OverloadedWithTwoParameters
        {
            readonly int _i;

            OverloadedWithTwoParameters(int i)
            {
                _i = i;
            }

            public bool Equals(object red, object black)
            {
                return red == null ? black == null : red.Equals(black);
            }

            public int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        public sealed class OverloadedWithNoParameter
        {
            readonly int _i;

            OverloadedWithNoParameter(int i)
            {
                _i = i;
            }

            public bool Equals()
            {
                return false;
            }

            public int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        public sealed class OverloadedWithUnrelatedParameter
        {
            readonly int _i;

            OverloadedWithUnrelatedParameter(int i)
            {
                _i = i;
            }

            public bool Equals(int i)
            {
                return _i == i;
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        public sealed class OverloadedAndOverridden
        {
            readonly int _i;

            OverloadedAndOverridden(int i)
            {
                _i = i;
            }

            public override bool Equals(object obj)
            {
                var other = obj as OverloadedAndOverridden;
                return other != null && _i == other._i;
            }

            public bool Equals(OverloadedAndOverridden obj)
            {
                return obj != null && _i == obj._i;

            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        public sealed class NoEqualsMethod
        {
            readonly int _i;

            public NoEqualsMethod(int i)
            {
                _i = i;
            }

            public int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }
        #pragma warning restore 108
        #pragma warning restore 114
        #pragma warning restore 414
    }
}

