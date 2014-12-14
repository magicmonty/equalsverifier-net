using System;
using EqualsVerifier.TestHelpers;
using NUnit.Framework;
using EqualsVerifier.TestHelpers.Types;

namespace EqualsVerifier.Integration.ExtraFeatures
{
    [TestFixture]
    public class WarningsMixTest : IntegrationTestBase
    {

        [Test]
        public void GivenOnlyStrictInheritanceWarningIsSuppressed_WhenFieldsAreNonfinalAndClassIsNonfinal_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier
                        .ForType<MutablePoint>()
                        .Suppress(Warning.STRICT_INHERITANCE)
                        .Verify(),
                "Mutability:");
        }

        [Test]
        public void GivenOnlyNonfinalFieldsWarningIsSuppressed_WhenFieldsAreNonFinalAndClassIsNonFinal_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<MutablePoint>().Suppress(Warning.NONFINAL_FIELDS).Verify(),
                "Subclass:");
        }

        [Test]
        public void GivenBothStrictInheritanceAndNonfinalFieldsWarningsAreSuppressed_WhenFieldsAreNonfinalAndClassIsNonfinal_ThenSucceed()
        {
            EqualsVerifier
                .ForType<MutablePoint>()
                .Suppress(Warning.STRICT_INHERITANCE, Warning.NONFINAL_FIELDS)
                .Verify();
        }

        [Test]
        public void GivenOnlyStrictInheritanceWarningIsSuppressed_WhenClassIsNonfinalAndEqualsDoesNotCheckNull_ThenFail()
        {
            ExpectFailureWithCause<NullReferenceException>(
                () => EqualsVerifier.ForType<NeverNullColorContainer>().Suppress(Warning.STRICT_INHERITANCE).Verify(),
                "Non-nullity:");
        }

        [Test]
        public void GivenOnlyNullFieldsWarningIsSuppressed_WhenClassIsNonfinalAndEqualsDoesNotCheckNull_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<NeverNullColorContainer>().Suppress(Warning.NULL_FIELDS).Verify(),
                "Subclass:");
        }

        [Test]
        public void GivenBothStrictInheritanceAndNullFieldsWarningsAreSuppressed_WhenClassIsNonfinalAndEqualsDoesNotCheckNull_ThenSucceed()
        {
            EqualsVerifier
                .ForType<NeverNullColorContainer>()
                .Suppress(Warning.STRICT_INHERITANCE, Warning.NULL_FIELDS)
                .Verify();
        }

        [Test]
        public void GivenOnlyStrictInheritanceAndNullFieldsWarningsAreSuppressed_WhenClassIsNonfinalAndFieldsAreNonfinalAndEqualsDoesNotCheckNull_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<NeverNullAndMutableColorContainer>().Suppress(
                    Warning.STRICT_INHERITANCE,
                    Warning.NULL_FIELDS).Verify(),
                "Mutability:");
        }

        [Test]
        public void fail_whenClassIsNonfinalAndFieldsAreNonfinalAndEqualsDoesNotCheckNull_givenOnlyStrictInheritanceAndNonfinalFieldsWarningsAreSuppressed()
        {
            ExpectFailureWithCause<NullReferenceException>(
                () => EqualsVerifier.ForType<NeverNullAndMutableColorContainer>().Suppress(
                    Warning.STRICT_INHERITANCE,
                    Warning.NONFINAL_FIELDS).Verify(),
                "Non-nullity:");
        }

        [Test]
        public void GivenOnlyNonfinalFieldsAndNullFieldsWarningsAreSuppressed_WhenClassIsNonfinalAndFieldsAreNonfinalAndEqualsDoesNotCheckNull_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<NeverNullAndMutableColorContainer>().Suppress(
                    Warning.NONFINAL_FIELDS,
                    Warning.NULL_FIELDS).Verify(),
                "Subclass:");
        }

        [Test]
        public void GivenAllNecessaryWarningsAreSuppressed_WhenClassIsNonfinalAndFieldsAreNonfinalAndEqualsDoesNotCheckNull_ThenSucceed()
        {
            EqualsVerifier
                .ForType<NeverNullAndMutableColorContainer>()
                .Suppress(Warning.STRICT_INHERITANCE, Warning.NULL_FIELDS, Warning.NONFINAL_FIELDS)
                .Verify();
        }

        public class MutablePoint
        {
            int _x;
            int _y;

            public MutablePoint(int x, int y)
            {
                _x = x;
                _y = y;
            }

            public override bool Equals(object obj)
            {
                var other = obj as MutablePoint;
                return other != null && _x == other._x && _y == other._y;
            }

            public override int GetHashCode()
            {
                return _x + (31 * _y);
            }
        }

        public class NeverNullColorContainer
        {
            readonly object _color;

            public NeverNullColorContainer(object color)
            {
                _color = color;
            }

            public override bool Equals(object obj)
            {
                var other = obj as NeverNullColorContainer;
                return other != null && _color == other._color;
            }

            public override int GetHashCode()
            {
                return _color.GetHashCode();
            }
        }

        public class NeverNullAndMutableColorContainer
        {
            object _color;

            public NeverNullAndMutableColorContainer(object color)
            {
                _color = color;
            }

            public override bool Equals(object obj)
            {
                var other = obj as NeverNullAndMutableColorContainer;
                return other != null && _color == other._color;
            }

            public override int GetHashCode()
            {
                return _color.GetHashCode();
            }
        }
    }
}

