using System;
using EqualsVerifier.TestHelpers;
using NUnit.Framework;
using EqualsVerifier.TestHelpers.Types;

namespace EqualsVerifier.Integration.Inheritance
{
    [TestFixture]
    public class AbstractHierarchyTest : IntegrationTestBase
    {
        [Test]
        public void GivenClassIsAbstract_WhenEqualsAndHashCodeAreFinal_ThenSucceed()
        {
            EqualsVerifier
                .ForType<AbstractFinalMethodsPoint>()
                .Verify();
        }

        [Test]
        public void GivenClassIsAbstract_WhenAnImplementingClassWithCorrectlyImplementedEquals_ThenSucceed()
        {
            EqualsVerifier
                .ForType<AbstractRedefinablePoint>()
                .WithRedefinedSubclass<SealedRedefinedColorPoint>()
                .Verify();
        }

        [Test]
        public void GivenClassIsAbstract_WhenEqualsThrowsNull_ThenFail()
        {
            ExpectFailureWithCause<NullReferenceException>(
                () => EqualsVerifier.ForType<NullThrowingColorContainer>().Verify(),
                "Non-nullity: equals throws NullReferenceException");
        }

        [Test]
        public void GivenClassIsAbstractAndWarningIsSuppressed_WhenEqualsThrowsNull_ThenSucceed()
        {
            EqualsVerifier
                .ForType<NullThrowingColorContainer>()
                .Suppress(Warning.NULL_FIELDS)
                .Verify();
        }

        public abstract class AbstractFinalMethodsPoint
        {
            readonly int _x;
            readonly int _y;

            public AbstractFinalMethodsPoint(int x, int y)
            { 
                _x = x; 
                _y = y; 
            }

            public override sealed bool Equals(object obj)
            {
                var other = obj as AbstractFinalMethodsPoint;
                return other != null && _x == other._x && _y == other._y;
            }

            public override sealed int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        public abstract class AbstractRedefinablePoint
        {
            readonly int _x;
            readonly int _y;

            public AbstractRedefinablePoint(int x, int y)
            { 
                _x = x; 
                _y = y; 
            }

            public virtual bool CanEqual(object obj)
            {
                return obj is AbstractRedefinablePoint;
            }

            public override bool Equals(object obj)
            {
                var other = obj as AbstractRedefinablePoint;
                return other != null
                && other.CanEqual(this)
                && _x == other._x
                && _y == other._y;
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        public sealed class SealedRedefinedColorPoint : AbstractRedefinablePoint
        {
            readonly Color _color;

            public SealedRedefinedColorPoint(int x, int y, Color color) : base(x, y)
            { 
                _color = color; 
            }

            public override bool CanEqual(object obj)
            {
                return obj is SealedRedefinedColorPoint;
            }

            public override bool Equals(object obj)
            {
                var other = obj as SealedRedefinedColorPoint;
                return other != null
                && other.CanEqual(this)
                && base.Equals(other)
                && _color == other._color;
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        public abstract class NullThrowingColorContainer
        {
            readonly object _color;

            public NullThrowingColorContainer(object color)
            {
                _color = color;
            }

            public override sealed bool Equals(object obj)
            {
                var other = obj as NullThrowingColorContainer;
                return other != null && _color.Equals(other._color);
            }

            public override sealed int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }
    }
}

