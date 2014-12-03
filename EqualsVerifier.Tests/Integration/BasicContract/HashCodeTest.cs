using EqualsVerifier.TestHelpers;
using NUnit.Framework;
using EqualsVerifier.TestHelpers.Types;

namespace EqualsVerifier.Integration.BasicContract
{
    [TestFixture]
    public class HashCodeTest : IntegrationTestBase
    {
        [Test, Ignore]
        public void GivenEqualObjects_WhenHashCodesAreUnequal_ThenFail()
        {
            ExpectFailure(
                () => {
                    EqualsVerifier
                        .ForType(typeof(RandomHashCode))
                        .Verify();
                },
                "hashCode: hashCodes should be equal", 
                typeof(RandomHashCode).Name);
        }

        public class RandomHashCode : Point
        {
            public RandomHashCode(int x, int y) : base(x, y)
            {
            }

            public override int GetHashCode()
            {
                return new object().GetHashCode();
            }
        }
    }
}

