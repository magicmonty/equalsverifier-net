using EqualsVerifier.TestHelpers;
using NUnit.Framework;

namespace EqualsVerifier.Integration.BasicContract
{
    [TestFixture]
    public class SymmetryTest : IntegrationTestBase
    {
        const string SYMMETRY = "Symmetry";
        const string NOT_SYMMETRIC = "objects are not symmetric";
        const string AND = "and";

        [Test]
        public void WhenEqualsIsNotSymmetrical_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<SymmetryIntentionallyBroken>().Verify(),
                SYMMETRY, NOT_SYMMETRIC, AND, typeof(SymmetryIntentionallyBroken).Name);
        }

        sealed class SymmetryIntentionallyBroken
        {
            readonly int _x;
            readonly int _y;

            public SymmetryIntentionallyBroken(int x, int y)
            {
                _x = x;
                _y = y;
            }

            public override bool Equals(object obj)
            {
                if (GoodEquals(obj))
                    return true;

                if (ReferenceEquals(obj, null))
                    return false;

                return GetHashCode() > obj.GetHashCode();
            }

            public bool GoodEquals(object obj)
            {
                var p = obj as SymmetryIntentionallyBroken;
                return p != null && p._x == _x && p._y == _y;
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }
    }
}

