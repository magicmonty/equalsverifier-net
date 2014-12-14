using System;
using EqualsVerifier.TestHelpers;
using NUnit.Framework;
using EqualsVerifier.TestHelpers.Types;

namespace EqualsVerifier.Integration.Inheritance
{
    [TestFixture]
    public class SuperclassTest : IntegrationTestBase
    {
        [Test]
        public void GivenSuperHasRedefinedAlso_WhenSubclassRedefinesEqualsButOnlyCallsSuper_ThenSucceed()
        {
            EqualsVerifier
                .ForType<ColorBlindColorPoint>()
                .Verify();
        }

        [Test]
        public void GivenSuperHasRedefinedAlso_WhenEqualsIsRedefinedSoItBreaksSymmetry_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<SymmetryBrokenColorPoint>().Verify(),
                "Symmetry", 
                typeof(SymmetryBrokenColorPoint).Name, 
                "does not equal superclass instance", 
                typeof(Point).Name);
        }

        [Test]
        public void GivenSuperHasRedefinedAlso_WhenEqualsIsRedefinedSoItBreaksTransitivity_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<TransitivityBrokenColorPoint>().Verify(),
                "Transitivity", 
                typeof(TransitivityBrokenColorPoint).Name,
                "both equal superclass instance", 
                typeof(Point).Name,
                "which implies they equal each other.");
        }

        [Test]
        public void GivenEqualsIsTheSame_WhenClassHasDifferentHashCodeThanSuper_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<GetHashCodeBrokenPoint>().Verify(),
                "Superclass", 
                "GetHashCode for", 
                typeof(GetHashCodeBrokenPoint).Name,
                "should be equal to GetHashCode for superclass instance", 
                typeof(Point).Name);
        }

        [Test]
        public void WhenSuperDoesNotRedefineEquals_ThenSucceed()
        {
            EqualsVerifier.ForType<SubclassOfEmpty>().Verify();
            EqualsVerifier.ForType<SubOfEmptySubOfEmpty>().Verify();
            EqualsVerifier.ForType<SubOfEmptySubOfAbstract>().Verify();
        }

        [Test]
        public void GivenSuperOfSuperDoesRedefineEquals_WhenSuperDoesNotRedefineEquals_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<BrokenCanEqualColorPointWithEmptySuper>().Verify(),
                "Symmetry", 
                typeof(BrokenCanEqualColorPointWithEmptySuper).Name);
        }

        [Test]
        public void GivenItIsNotNeeded_WhenWithRedefinedSuperclassIsUsed_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<ColorBlindColorPoint>().WithRedefinedSuperclass().Verify(),
                "Redefined superclass", 
                typeof(ColorBlindColorPoint).Name,
                "should not equal superclass instance", 
                typeof(Point).Name, 
                "but it does");
        }

        public class SymmetryBrokenColorPoint : Point
        {
            readonly Color _color;

            public SymmetryBrokenColorPoint(int x, int y, Color color) : base(x, y)
            {
                _color = color;
            }

            public override bool Equals(object obj)
            {
                var other = obj as SymmetryBrokenColorPoint;
                return other != null
                && base.Equals(obj)
                && other._color == _color;
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        public class TransitivityBrokenColorPoint : Point
        {
            readonly Color _color;

            public TransitivityBrokenColorPoint(int x, int y, Color color) : base(x, y)
            {
                _color = color;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is Point))
                    return false;

                var other = obj as TransitivityBrokenColorPoint;
                return other == null 
                    ? Object.Equals(obj, this) 
                    : base.Equals(obj) && other._color == _color;

            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        public class GetHashCodeBrokenPoint : Point
        {
            public GetHashCodeBrokenPoint(int x, int y) : base(x, y)
            {
            }

            public override int GetHashCode()
            {
                return base.GetHashCode() + 1;
            }
        }

        public sealed class SubclassOfEmpty : TypeHelper.Empty
        {
            readonly Color _color;

            public SubclassOfEmpty(Color color)
            {
                _color = color;
            }

            public override bool Equals(object obj)
            {
                var other = obj as SubclassOfEmpty;
                return other != null && _color == other._color;
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        public class EmptySubOfEmpty : TypeHelper.Empty
        {

        }

        public sealed class SubOfEmptySubOfEmpty : EmptySubOfEmpty
        {
            readonly Color _color;

            public SubOfEmptySubOfEmpty(Color color)
            {
                _color = color;
            }

            public override bool Equals(object obj)
            {
                var other = obj as SubOfEmptySubOfEmpty;
                return other != null && _color == other._color;
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        public abstract class EmptySubOfAbstract : TypeHelper.AbstractEqualsAndHashCode
        {

        }

        public sealed class SubOfEmptySubOfAbstract : EmptySubOfAbstract
        {
            readonly Color _color;

            public SubOfEmptySubOfAbstract(Color color)
            {
                _color = color;
            }

            public override bool Equals(object obj)
            {
                var other = obj as SubOfEmptySubOfAbstract;
                return other != null
                && _color == other._color;
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        public class EmptySubOfCanEqualPoint : CanEqualPoint
        {
            public EmptySubOfCanEqualPoint(int x, int y) : base(x, y)
            {
            }
        }

        public sealed class BrokenCanEqualColorPointWithEmptySuper : EmptySubOfCanEqualPoint
        {
            readonly Color _color;

            public BrokenCanEqualColorPointWithEmptySuper(int x, int y, Color color) : base(x, y)
            {
                _color = color;
            }

            public override bool Equals(object obj)
            {
                var other = obj as BrokenCanEqualColorPointWithEmptySuper;
                return other != null
                && base.Equals(other)
                && _color == other._color;
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }
    }
}

