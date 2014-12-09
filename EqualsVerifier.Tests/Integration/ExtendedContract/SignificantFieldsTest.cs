using EqualsVerifier.TestHelpers;
using NUnit.Framework;
using System;
using EqualsVerifier.TestHelpers.Types;


namespace EqualsVerifier.Integration.ExtendedContract
{
    [TestFixture]
    public class SignificantFieldsTest : IntegrationTestBase
    {
        [Test]
        public void WhenEqualsUsesAFieldAndHashCodeDoesnt_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<ExtraFieldInEquals>().Verify(),
                "Significant fields", 
                "Equals relies on", 
                "_yNotUsed", 
                "but GetHashCode does not");
        }

        [Test]
        public void WhenHashCodeUsesAFieldAndEqualsDoesnt_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<ExtraFieldInHashCode>().Verify(),
                "Significant fields", 
                "GetHashCode relies on", 
                "_yNotUsed", 
                "but Equals does not");
        }

        [Test]
        public void GivenAllFieldsShouldBeUsed_WhenAllFieldsAreUsed_ThenSucceed()
        {
            EqualsVerifier
                .ForType<SealedPoint>()
                .AllFieldsShouldBeUsed()
                .Verify();
        }

        [Test]
        public void WhenAFieldIsUnused_ThenSucceed()
        {
            EqualsVerifier
                .ForType<OneFieldUnused>()
                .Verify();
        }

        [Test]
        public void GivenAllFieldsShouldBeUsed_WhenAFieldIsUnused_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<OneFieldUnused>().AllFieldsShouldBeUsed().Verify(),
                "Significant fields", 
                "Equals does not use", 
                "_colorNotUsed");
        }

        [Test]
        public void GivenAllFieldsShouldBeUsed_WhenAStaticFieldIsUnused_ThenSucceed()
        {
            EqualsVerifier
                .ForType<OneStaticFieldUnusedColorPoint>()
                .AllFieldsShouldBeUsed()
                .Verify();
        }

        [Test]
        public void WhenAFieldIsUnusedInASubclass_ThenSucceed()
        {
            EqualsVerifier
                .ForType<OneFieldUnusedExtended>()
                .Verify();
        }

        [Test]
        public void GivenAllFieldsShouldBeUsed_WhenAFieldIsUnusedInASubclass_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<OneFieldUnusedExtended>().AllFieldsShouldBeUsed().Verify(),
                "Significant fields", 
                "Equals does not use", 
                "_colorNotUsed");
        }

        [Test]
        public void WhenNoFieldsAreUsed_ThenSucceed()
        {
            EqualsVerifier
                .ForType<NoFieldsUsed>()
                .Verify();
        }

        [Test]
        public void GivenAllFieldsShouldBeUsed_WhenNoFieldsAreAdded_ThenSucceed()
        {
            EqualsVerifier
                .ForType<NoFieldsAdded>()
                .AllFieldsShouldBeUsed()
                .Verify();
        }

        [Test]
        public void GivenAllFieldsShouldBeUsed_WhenNoFieldsAreUsed_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<NoFieldsUsed>().AllFieldsShouldBeUsed().Verify(),
                "Significant fields", 
                "all fields should be used", 
                "NoFieldsUsed", 
                "has not defined an Equals method");
        }

        [Test]
        public void GivenAllFieldsShouldBeUsedExceptThatField_WhenAFieldIsUnused_ThenSucceed()
        {
            EqualsVerifier
                .ForType<OneFieldUnused>()
                .AllFieldsShouldBeUsedExcept("_colorNotUsed")
                .Verify();
        }

        [Test]
        public void GivenAllFieldsShouldBeUsedExceptThoseTwo_WhenTwoFieldsAreUnused_ThenSucceed()
        {
            EqualsVerifier
                .ForType<TwoFieldsUnusedColorPoint>()
                .AllFieldsShouldBeUsedExcept("_colorNotUsed", "_colorAlsoNotUsed")
                .Verify();
        }

        [Test]
        public void GivenAllFieldsShouldBeUsedExceptOneOfThemButNotBoth_WhenTwoFieldsAreUnUsed_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<TwoFieldsUnusedColorPoint>().AllFieldsShouldBeUsedExcept("_colorNotUsed").Verify(),
                "Significant fields", 
                "Equals does not use", 
                "_colorAlsoNotUsed");
        }

        [Test]
        public void GivenAllFieldsShouldBeUsedExceptOneThatActuallyIsUsed_WhenAllFieldsAreUsed_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<SealedPoint>().AllFieldsShouldBeUsedExcept("_x").Verify(),
                "Significant fields", 
                "Equals should not use", 
                "_x", 
                "but it does");
        }

        [Test]
        public void GivenAllFieldsShouldBeUsedExceptTwoFields_WhenOneFieldIsUnused_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<OneFieldUnused>().AllFieldsShouldBeUsedExcept("_x", "_colorNotUsed").Verify(),
                "Significant fields", 
                "Equals should not use", 
                "_x", 
                "but it does");
        }

        [Test]
        public void WhenANonExistingFieldIsExcepted_AnExceptionIsThrown()
        {
            ExpectException<ArgumentException>(
                () => EqualsVerifier.ForType<SealedPoint>().AllFieldsShouldBeUsedExcept("thisFieldDoesNotExist"),
                "Class SealedPoint does not contain field thisFieldDoesNotExist.");
        }

        [Test]
        public void WhenAUsedFieldHasUnusedStaticFinalMembers_ThenSucceed()
        {
            EqualsVerifier
                .ForType<IndirectStaticFinalContainer>()
                .Verify();
        }

        #pragma warning disable 414
        public sealed class ExtraFieldInEquals
        {
            readonly int _x;
            readonly int _yNotUsed;

            public ExtraFieldInEquals(int x, int y)
            {
                this._x = x;
                this._yNotUsed = y;
            }

            public override bool Equals(object obj)
            {
                var other = obj as ExtraFieldInEquals;
                return other != null && other._x == _x && other._yNotUsed == _yNotUsed;
            }

            public override int GetHashCode()
            {
                return _x;
            }
        }

        public sealed class ExtraFieldInHashCode
        {
            readonly int _x;
            readonly int _yNotUsed;

            public ExtraFieldInHashCode(int x, int y)
            {
                _x = x;
                _yNotUsed = y;
            }

            public override bool Equals(object obj)
            {
                var other = obj as ExtraFieldInHashCode;
                return other != null && other._x == _x;
            }

            public override int GetHashCode()
            {
                return _x + (31 * _yNotUsed);
            }
        }

        public sealed class OneFieldUnused
        {
            readonly int _x;
            readonly int _y;
            readonly Color _colorNotUsed;

            public OneFieldUnused(int x, int y, Color color)
            {
                _x = x;
                _y = y;
                _colorNotUsed = color;
            }

            public override bool Equals(object obj)
            {
                var other = obj as OneFieldUnused;
                return other != null && _x == other._x && _y == other._y;
            }

            public override int GetHashCode()
            {
                return _x + (31 * _y);
            }
        }

        public sealed class OneStaticFieldUnusedColorPoint
        {
            readonly int _x;
            readonly int _y;
            static Color color;

            public OneStaticFieldUnusedColorPoint(int x, int y, Color color)
            {
                _x = x;
                _y = y;
                OneStaticFieldUnusedColorPoint.color = color;
            }

            public override bool Equals(object obj)
            {
                var other = obj as OneStaticFieldUnusedColorPoint;
                return other != null && _x == other._x && _y == other._y;
            }

            public override int GetHashCode()
            {
                return _x + (31 * _y);
            }
        }

        public sealed class OneFieldUnusedExtended : Point
        {
            readonly Color _colorNotUsed;

            public OneFieldUnusedExtended(int x, int y, Color color) : base(x, y)
            {
                _colorNotUsed = color;
            }
        }

        public sealed class NoFieldsUsed
        {
            readonly Color _color;

            public NoFieldsUsed(Color color)
            {
                _color = color;
            }
        }

        public sealed class NoFieldsAdded : Point
        {
            public NoFieldsAdded(int x, int y) : base(x, y)
            {
            }
        }

        public sealed class TwoFieldsUnusedColorPoint
        {
            readonly int _x;
            readonly int _y;
            readonly Color _colorNotUsed;
            readonly Color _colorAlsoNotUsed;

            public TwoFieldsUnusedColorPoint(int x, int y, Color color)
            {
                _x = x;
                _y = y;
                _colorNotUsed = color;
                _colorAlsoNotUsed = color;
            }

            public override bool Equals(object obj)
            {
                var other = obj as TwoFieldsUnusedColorPoint;
                return other != null && _x == other._x && _y == other._y;
            }

            public override int GetHashCode()
            {
                return _x + (31 * _y);
            }
        }

        public sealed class X
        {
            public static readonly X x = new X();
        }

        public sealed class IndirectStaticFinalContainer
        {
            readonly X _x;

            public IndirectStaticFinalContainer(X x)
            {
                _x = x;
            }

            public override bool Equals(object obj)
            {
                var other = obj as IndirectStaticFinalContainer;
                return other != null && other._x == _x;
            }

            public override int GetHashCode()
            {
                return _x.GetNullSafeHashCode();
            }
        }
        #pragma warning restore 414
    }
}

