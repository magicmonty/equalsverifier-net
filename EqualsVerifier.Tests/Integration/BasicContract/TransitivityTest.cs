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
    }
}

