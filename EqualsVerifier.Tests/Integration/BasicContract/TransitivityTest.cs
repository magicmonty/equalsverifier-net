using EqualsVerifier.TestHelpers;
using NUnit.Framework;

namespace EqualsVerifier.Integration.BasicContract
{
    [TestFixture]
    public class TransitivityTest : IntegrationTestBase
    {
        [Test]
        public void WhenEqualityForTwoFieldsIsCombinedUsingAND_ThenSucceed()
        {
            EqualsVerifier.ForType<TwoFieldsUsingAND>().Verify();
        }

        [Test]
        public void WhenEqualityForTwoFieldsIsCombinedUsingOR_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<TwoFieldsUsingOR>().Verify(),
                "Transitivity",
                "two of these three instances are equal to each other, so the third one should be, too",
                typeof(TwoFieldsUsingOR).Name);
        }

        sealed class TwoFieldsUsingAND
        {
            readonly string _f;
            readonly string _g;

            public TwoFieldsUsingAND(string f, string g)
            {
                _f = f;
                _g = g;
            }

            public override bool Equals(object obj)
            {
                var other = obj as TwoFieldsUsingAND;

                return 
                    other != null
                && _f.NullSafeEquals(other._f)
                && _g.NullSafeEquals(other._g);
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        sealed class TwoFieldsUsingOR
        {
            readonly string f;
            readonly string g;

            public TwoFieldsUsingOR(string f, string g)
            {
                this.f = f;
                this.g = g;
            }

            public override bool Equals(object obj)
            {
                var other = obj as TwoFieldsUsingOR;
                if (other == null)
                    return false;

                return f.NullSafeEquals(other.f) || g.NullSafeEquals(other.g);
            }

            public override int GetHashCode()
            {
                return 42;
            }
        }
    }
}

