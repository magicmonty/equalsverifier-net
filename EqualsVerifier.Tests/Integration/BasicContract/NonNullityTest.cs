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
        #pragma warning restore 659
    }
}

