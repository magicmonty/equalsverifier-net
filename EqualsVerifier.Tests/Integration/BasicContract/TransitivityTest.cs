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

        [Test]
        public void WhenEqualityForThreeFieldsIsCombinedUsingAND_ThenSucceed()
        {
            EqualsVerifier.ForType<ThreeFieldsUsingAND>().Verify();
        }

        [Test]
        public void WhenEqualityForThreeFieldsIsCombinedUsingOR_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<ThreeFieldsUsingOR>().Verify(),
                "Transitivity");
        }

        [Test]
        public void GivenRelaxedEqualExamples_WhenEqualityForThreeFieldsIsCombinedUsingOR_ThenFail()
        {
            var one = new ThreeFieldsUsingOR("a", "1", "alpha");
            var two = new ThreeFieldsUsingOR("b", "1", "alpha");
            var three = new ThreeFieldsUsingOR("c", "1", "alpha");
            var other = new ThreeFieldsUsingOR("d", "4", "delta");

            ExpectFailure(
                () => EqualsVerifier
                .ForRelaxedEqualExamples(one, two, three)
                .AndUnequalExample(other)
                .Verify(),
                "Transitivity");
        }

        [Test]
        public void WhenEqualityForThreeFieldsIsCombinedUsingANDAndOR_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<ThreeFieldsUsingANDOR>().Verify(),
                "Transitivity");
        }

        [Test]
        public void WhenEqualityForThreeFieldsIsCombinedUsingORAndAND_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<ThreeFieldsUsingORAND>().Verify(),
                "Transitivity");
        }

        [Test]
        public void WhenEqualityForFiveFieldsIsCombinedUsingOR_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<FiveFieldsUsingOR>().Verify(),
                "Transitivity");
        }

        [Test]
        public void WhenEqualityForFiveFieldsIsCombinedUsingANDsAndORs_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<FiveFieldsUsingANDsAndORs>().Verify(),
                "Transitivity");
        }

        [Test]
        [Ignore("This class is not transitive, and it should fail. See issue 78 for the original EqualsVerifier on Github.")]
        public void WhenInstancesAreEqualIfAtLeastTwoFieldsAreEqual_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<AtLeast2FieldsAreEqual>().Verify(),
                "Transitivity");
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
                return other != null
                && (
                    f.NullSafeEquals(other.f)
                    || g.NullSafeEquals(other.g));

            }

            public override int GetHashCode()
            {
                return 42;
            }
        }

        sealed class ThreeFieldsUsingAND
        {
            readonly string _f;
            readonly string _g;
            readonly string _h;

            public ThreeFieldsUsingAND(string f, string g, string h)
            {
                _f = f;
                _g = g;
                _h = h;
            }

            public override bool Equals(object obj)
            {
                var other = obj as ThreeFieldsUsingAND;
                return other != null
                && _f.NullSafeEquals(other._f)
                && _g.NullSafeEquals(other._g)
                && _h.NullSafeEquals(other._h);
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        sealed class ThreeFieldsUsingOR
        {
            readonly string _f;
            readonly string _g;
            readonly string _h;

            public ThreeFieldsUsingOR(string f, string g, string h)
            {
                _f = f;
                _g = g;
                _h = h;
            }

            public override bool Equals(object obj)
            {
                var other = obj as ThreeFieldsUsingOR;
                return other != null
                && (
                    _f.NullSafeEquals(other._f)
                    || _g.NullSafeEquals(other._g)
                    || _h.NullSafeEquals(other._h));
            }

            public override int GetHashCode()
            {
                return 42;
            }
        }

        sealed class ThreeFieldsUsingANDOR
        {
            readonly string _f;
            readonly string _g;
            readonly string _h;

            public ThreeFieldsUsingANDOR(string f, string g, string h)
            {
                _f = f;
                _g = g;
                _h = h;
            }

            public override bool Equals(object obj)
            {
                var other = obj as ThreeFieldsUsingANDOR;
                return other != null
                && (
                    _f.NullSafeEquals(other._f)
                    && _g.NullSafeEquals(other._g)
                    || _h.NullSafeEquals(other._h));
            }

            public override int GetHashCode()
            {
                return 42;
            }
        }

        sealed class ThreeFieldsUsingORAND
        {
            readonly string _f;
            readonly string _g;
            readonly string _h;

            public ThreeFieldsUsingORAND(string f, string g, string h)
            {
                _f = f;
                _g = g;
                _h = h;
            }

            public override bool Equals(object obj)
            {
                var other = obj as ThreeFieldsUsingORAND;
                return other != null && (
                    _f.NullSafeEquals(other._f)
                    || _g.NullSafeEquals(other._g)
                    && _h.NullSafeEquals(other._h));
            }

            public override int GetHashCode()
            {
                return 42;
            }
        }

        sealed class FiveFieldsUsingOR
        {
            readonly string _f;
            readonly string _g;
            readonly string _h;
            readonly string _i;
            readonly string _j;

            public FiveFieldsUsingOR(string f, string g, string h, string i, string j)
            {
                _f = f;
                _g = g;
                _h = h;
                _i = i;
                _j = j;
            }

            public override bool Equals(object obj)
            {
                var other = obj as FiveFieldsUsingOR;
                return other != null && (
                    _f.NullSafeEquals(other._f)
                    || _g.NullSafeEquals(other._g)
                    || _h.NullSafeEquals(other._h)
                    || _i.NullSafeEquals(other._i)
                    || _j.NullSafeEquals(other._j));
            }

            public override int GetHashCode()
            {
                return 42;
            }
        }

        sealed class FiveFieldsUsingANDsAndORs
        {
            readonly string _f;
            readonly string _g;
            readonly string _h;
            readonly string _i;
            readonly string _j;

            public FiveFieldsUsingANDsAndORs(string f, string g, string h, string i, string j)
            {
                _f = f;
                _g = g;
                _h = h;
                _i = i;
                _j = j;
            }

            public override bool Equals(object obj)
            {
                var other = obj as FiveFieldsUsingANDsAndORs;
                return other != null && (
                    _f.NullSafeEquals(other._f)
                    || _g.NullSafeEquals(other._g)
                    && _h.NullSafeEquals(other._h)
                    || _i.NullSafeEquals(other._i)
                    && _j.NullSafeEquals(other._j));
            }

            public override int GetHashCode()
            {
                return 42;
            }
        }

        sealed class AtLeast2FieldsAreEqual
        {
            readonly int _i;
            readonly int _j;
            readonly int _k;
            readonly int _l;

            public AtLeast2FieldsAreEqual(int i, int j, int k, int l)
            {
                _i = i;
                _j = j;
                _k = k;
                _l = l;
            }

            public override bool Equals(object obj)
            {
                var other = obj as AtLeast2FieldsAreEqual;
                if (other == null)
                    return false;

                int x = 0;
                if (_i == other._i)
                    x++;

                if (_j == other._j)
                    x++;

                if (_k == other._k)
                    x++;

                if (_l == other._l)
                    x++;

                return x >= 2;
            }

            public override int GetHashCode()
            {
                return 42;
            }
        }
    }
}

