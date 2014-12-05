using NUnit.Framework;
using EqualsVerifier.TestHelpers;
using EqualsVerifier.TestHelpers.Types;

namespace EqualsVerifier.Integration.BasicContract
{
    [TestFixture]
    public class NonNullityTest : IntegrationTestBase
    {
        [Test]
        public void GivenNullInput_WhenNullPointerExceptionIsThrown_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<NullReferenceExceptionThrower>().Verify(),
                "Non-nullity: NullReferenceException thrown");
        }

        [Test]
        public void GivenNullInput_WhenEqualsReturnsTrue_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<NullReturnsTrue>().Verify(),
                "Non-nullity: true returned for null value");
            ;
        }


        #pragma warning disable 659
        sealed class NullReferenceExceptionThrower : Point
        {
            public NullReferenceExceptionThrower(int x, int y) : base(x, y)
            {
            }

            public override bool Equals(object obj)
            {
                return obj.GetType().Equals(GetType()) && base.Equals(obj);
            }
        }

        sealed class NullReturnsTrue : Point
        {
            public NullReturnsTrue(int x, int y) : base(x, y)
            {
            }

            public override bool Equals(object obj)
            {
                return obj == null || base.Equals(obj);
            }
        }

        #pragma warning restore 659
    }
}

