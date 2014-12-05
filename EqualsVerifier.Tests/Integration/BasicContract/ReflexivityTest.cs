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
        #pragma warning restore 659
    }
}

