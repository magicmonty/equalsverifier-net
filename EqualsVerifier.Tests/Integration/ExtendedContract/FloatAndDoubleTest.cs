using System;
using NUnit.Framework;
using EqualsVerifier.TestHelpers;

namespace EqualsVerifier.Integration.ExtendedContract
{
    [TestFixture]
    public class FloatAndDoubleTest : IntegrationTestBase
    {
        const string FLOAT = "Float: Equals uses reference comparison for field";
        const string DOUBLE = "Double: Equals uses reference comparison for field";

        [Test]
        public void WhenFloatsAreComparedByReference_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<ComparePrimitiveFloatsByReference>().Verify(),
                FLOAT, 
                "_f");
        }

        [Test]
        public void WhenFloatsAreComparedWithFloatCompare_ThenSucceed()
        {
            EqualsVerifier
                .ForType<CompareFloatCorrectly>()
                .Verify();
        }

        [Test]
        public void WhenDoublesAreComparedByReference_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<ComparePrimitiveDoubleByReference>().Verify(),
                DOUBLE, 
                "_d");
        }

        [Test]
        public void WhenDoublesAreComparedWithDoubleCompare_ThenSucceed()
        {
            EqualsVerifier
                .ForType<CompareDoubleCorrectly>()
                .Verify();
        }

        public sealed class ComparePrimitiveFloatsByReference
        {
            readonly float _f;

            public ComparePrimitiveFloatsByReference(float f)
            {
                _f = f;
            }

            public override bool Equals(Object obj)
            {
                var other = obj as ComparePrimitiveFloatsByReference;
                return other != null
                && _f == other._f;
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        public sealed class CompareFloatCorrectly
        {
            readonly float _f;

            public CompareFloatCorrectly(float f)
            {
                _f = f;
            }

            public override bool Equals(Object obj)
            {
                var other = obj as CompareFloatCorrectly;
                return other != null
                && !((float.IsNaN(_f) && !float.IsNaN(other._f)) || (!float.IsNaN(_f) && float.IsNaN(other._f)))
                && (
                    (float.IsNaN(_f) && float.IsNaN(other._f))
                    || (Math.Abs(_f - other._f) <= float.Epsilon));
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        public sealed class ComparePrimitiveDoubleByReference
        {
            readonly double _d;

            public ComparePrimitiveDoubleByReference(double d)
            {
                _d = d;
            }

            public override bool Equals(Object obj)
            {
                var other = obj as ComparePrimitiveDoubleByReference;
                return other != null
                && _d == other._d;
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        public sealed class CompareDoubleCorrectly
        {
            readonly double _d;

            public CompareDoubleCorrectly(double d)
            {
                _d = d;
            }

            public override bool Equals(Object obj)
            {
                var other = obj as CompareDoubleCorrectly;
                return other != null
                && !((Double.IsNaN(_d) && !Double.IsNaN(other._d)) || (!Double.IsNaN(_d) && Double.IsNaN(other._d)))
                && (
                    (Double.IsNaN(_d) && Double.IsNaN(other._d))
                    || (Math.Abs(_d - other._d) <= Double.Epsilon));
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }
    }
}

