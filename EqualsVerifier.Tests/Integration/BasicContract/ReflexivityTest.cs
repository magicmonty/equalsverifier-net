﻿using NUnit.Framework;
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

        #pragma warning disable 659
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
        #pragma warning restore 659
    }
}

