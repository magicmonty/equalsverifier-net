using NUnit.Framework;
using EqualsVerifier.TestHelpers;
using EqualsVerifier.TestHelpers.Types;

namespace EqualsVerifier.Integration.BasicContract
{
    [TestFixture]
    public class ReflexivityTest : IntegrationTestBase
    {
        [Test]
        public void WhenReferencesAreNotEqual_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<ReflexivityIntentionallyBroken>().Verify(),
                "Reflexivity", "object does not equal itself", typeof(ReflexivityIntentionallyBroken).Name);
        }

        [Test]
        public void WhenTheWrongFieldsAreComparedInEquals_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<FieldsMixedUpInEquals>().Verify(),
                "Reflexivity", "object does not equal an identical copy of itself", typeof(FieldsMixedUpInEquals).Name);

        }

        [Test]
        public void GivenFieldsThatAreNull_WhenReferencesAreNotEqual_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<ReflexivityBrokenOnNullFields>().Verify(),
                "Reflexivity", typeof(ReflexivityBrokenOnNullFields).Name);
        }

        [Test]
        public void GivenFieldsThatAreNullAndWarningIsSuppressed_WhenReferencesAreNotEqual_ThenSucceed()
        {
            EqualsVerifier.ForType<ReflexivityBrokenOnNullFields>()
                .Suppress(Warning.NULL_FIELDS)
                .Verify();
        }

        [Test]
        public void WhenObjectTypeIsCheckedAgainstWrongType_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<WrongTypeCheck>().Verify(),
                "Reflexivity", "object does not equal an identical copy of itself", typeof(WrongTypeCheck).Name);
        }

        [Test]
        public void GivenObjectsThatAreIdentical_WhenEqualsReturnsFalse_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<SuperCallerWithUnusedField>().Verify(),
                "Reflexivity", "identical copy");
        }

        [Test]
        public void GivenObjectsThatAreIdenticalAndWarningIsSuppressed_WhenEqualsReturnsFalse_ThenSucceed()
        {
            EqualsVerifier.ForType<SuperCallerWithUnusedField>()
                .Suppress(Warning.IDENTICAL_COPY)
                .Verify();
        }

        [Test]
        public void WhenIdenticalCopyWarningIsSuppressedUnnecessarily_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<SealedPoint>().Suppress(Warning.IDENTICAL_COPY).Verify(),
                "Unnecessary suppression", "IDENTICAL_COPY");

        }


        #pragma warning disable 659
        #pragma warning disable 414
        sealed class ReflexivityIntentionallyBroken : Point
        {
            // Instantiator.scramble will flip this boolean.
            public bool Broken = false;

            public ReflexivityIntentionallyBroken(int x, int y) : base(x, y)
            {
            }

            public override bool Equals(object obj)
            {
                if (Broken && this == obj)
                    return false;

                return base.Equals(obj);
            }
        }

        sealed class FieldsMixedUpInEquals
        {
            string _one;
            string _two;
            string _unused;

            public FieldsMixedUpInEquals(string one, string two, string unused)
            { 
                this._one = one; 
                this._two = two; 
                this._unused = unused; 
            }

            public override bool Equals(object obj)
            {
                // EV must also find the error when equals short-circuits.
                if (ReferenceEquals(obj, this))
                    return true;

                var other = obj as FieldsMixedUpInEquals;
                if (ReferenceEquals(null, other))
                    return false;

                return _two.NullSafeEquals(other._one)
                && _two.NullSafeEquals(other._two);
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        sealed class ReflexivityBrokenOnNullFields
        {
            readonly object _a;

            public ReflexivityBrokenOnNullFields(object a)
            {
                _a = a;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(this, obj))
                    return true;

                if (ReferenceEquals(null, obj))
                    return false;

                if (GetType() != obj.GetType())
                    return false;

                var other = (ReflexivityBrokenOnNullFields)obj;
                if (_a == null)
                {
                    if (other._a != null)
                        return false;

                    // The following line was added to cause equals to be broken on reflexivity.
                    return false;
                }

                return _a.Equals(other._a);

            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode(); 
            }
        }

        sealed class WrongTypeCheck
        {
            readonly int _foo;

            public WrongTypeCheck(int foo)
            {
                _foo = foo;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(this, obj))
                    return true;

                if (!(obj is SomethingCompletelyDifferent))
                    return false;

                var other = (WrongTypeCheck)obj;
                return _foo == other._foo;
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        class SomethingCompletelyDifferent
        {

        }

        sealed class SuperCallerWithUnusedField
        {
            readonly int _unused;

            public SuperCallerWithUnusedField(int unused)
            {
                _unused = unused;
            }

            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }
        #pragma warning restore 414
        #pragma warning restore 659
    }
}

