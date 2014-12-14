using EqualsVerifier.TestHelpers;
using NUnit.Framework;
using EqualsVerifier.TestHelpers.Types;

namespace EqualsVerifier.Integration.Inheritance
{
    [TestFixture]
    public class SubclassTest : IntegrationTestBase
    {
        [Test]
        public void WhenClassIsSealed_ThenSucceed()
        {
            EqualsVerifier
                .ForType<SealedPoint>()
                .Verify();
        }

        [Test]
        public void WhenClassIsNotEqualToATrivialSubclassWithEqualFields_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<LiskovSubstitutionPrincipleBroken>().Verify(),
                "Subclass", 
                "object is not equal to an instance of a trivial subclass with equal fields", 
                "Consider making the class sealed.");
        }

        [Test]
        public void WhenEqualsIsOverridableAndBlindlyEqualsIsPresent_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier
                        .ForType<BlindlyEqualsPoint>()
                        .WithRedefinedSubclass<EqualSubclassForBlindlyEqualsPoint>()
                        .Verify(),
                "Subclass", 
                typeof(BlindlyEqualsPoint).Name, 
                "equals subclass instance", 
                typeof(EqualSubclassForBlindlyEqualsPoint).Name);
        }

        [Test]
        public void GivenACorrectSubclass_WhenEqualsIsOverridableAndBlindlyEqualsIsPresent_ThenSucceed()
        {
            EqualsVerifier
                .ForType<BlindlyEqualsPoint>()
                .WithRedefinedSubclass<BlindlyEqualsColorPoint>()
                .Verify();
        }

        [Test]
        public void GivenWithRedefinedSuperclass_WhenEqualsIsOverriddenTwiceThroughBlindlyEquals_ThenSucceed()
        {
            EqualsVerifier
                .ForType<BlindlyEqualsColorPoint>()
                .WithRedefinedSuperclass()
                .Verify();
        }

        [Test]
        public void WhenEqualsIsOverridableAndCanEqualIsPresent_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier
                        .ForType<CanEqualPoint>()
                        .WithRedefinedSubclass<EqualSubclassForCanEqualPoint>()
                        .Verify(),
                "Subclass", 
                typeof(CanEqualPoint).Name, 
                "equals subclass instance", 
                typeof(EqualSubclassForCanEqualPoint).Name);
        }

        [Test]
        public void GivenACorrectSubclass_WhenEqualsIsOverridableAndCanEqualIsPresent_ThenFail()
        {
            EqualsVerifier
                .ForType<CanEqualPoint>()
                .WithRedefinedSubclass<CanEqualColorPoint>()
                .Verify();
        }

        [Test]
        public void GivenWithRedefinedSuperclass_WhenEqualsIsOverridenTwiceThroughCanEqual_ThenSucceed()
        {
            EqualsVerifier
                .ForType<CanEqualColorPoint>()
                .WithRedefinedSuperclass()
                .Verify();
        }

        [Test]
        public void GivenEqualsAndGetHashCodeAreSealed_WhenWithRedefinedEqualsIsUsed_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier
                        .ForType<SealedEqualsAndHashCode>()
                        .WithRedefinedSubclass<RedeFinalSubPoint>()
                        .Verify(),
                "Subclass", 
                typeof(SealedEqualsAndHashCode).Name, 
                "has a sealed Equals method", 
                "No need to supply a redefined subclass");
        }

        [Test]
        public void GivenACorrectImplementationOfEqualsUnderInheritanceAndARedefinedSubclass_WhenClassIsAbstract_ThenSucceed()
        {
            EqualsVerifier
                .ForType<AbstractRedefinablePoint>()
                .WithRedefinedSubclass<SubclassForAbstractRedefinablePoint>()
                .Verify();
        }

        [Test]
        public void GivenStrictInheritanceWarningIsSuppressed_WhenWithRedefinedSubclassIsUsed()
        {
            ExpectFailure(
                () => EqualsVerifier
                        .ForType<CanEqualPoint>()
                        .Suppress(Warning.STRICT_INHERITANCE)
                        .WithRedefinedSubclass<EqualSubclassForCanEqualPoint>()
                        .Verify(),
                "WithRedefinedSubclass", 
                "weakInheritanceCheck", 
                "are mutually exclusive");
        }

        [Test]
        public void GivenWithRedefinedSubclassIsUsed_WhenStrictInhertianceWarningIsSuppressed_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier
                        .ForType<CanEqualPoint>()
                        .WithRedefinedSubclass<EqualSubclassForCanEqualPoint>()
                        .Suppress(Warning.STRICT_INHERITANCE)
                        .Verify(),
                "WithRedefinedSubclass", 
                "weakInheritanceCheck", 
                "are mutually exclusive");
        }

        public class LiskovSubstitutionPrincipleBroken
        {
            readonly int _x;

            public LiskovSubstitutionPrincipleBroken(int x)
            {
                _x = x;
            }

            public override sealed bool Equals(object obj)
            {
                return obj != null
                && GetType() == obj.GetType()
                && _x == ((LiskovSubstitutionPrincipleBroken)obj)._x;

            }

            public override sealed int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        public class SealedEqualsAndHashCode
        {
            readonly int _x;
            readonly int _y;

            public SealedEqualsAndHashCode(int x, int y)
            {
                _x = x;
                _y = y;
            }

            public override sealed bool Equals(object obj)
            {
                var other = obj as SealedEqualsAndHashCode;
                return other != null
                && _x == other._x
                && _y == other._y;
            }

            public override sealed int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        public sealed class RedeFinalSubPoint : SealedEqualsAndHashCode
        {
            public RedeFinalSubPoint(int x, int y) : base(x, y)
            {
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
                && other._x == _x
                && other._y == _y;
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        public sealed class SubclassForAbstractRedefinablePoint : AbstractRedefinablePoint
        {
            readonly Color _color;

            public SubclassForAbstractRedefinablePoint(int x, int y, Color color) : base(x, y)
            {
                _color = color;
            }

            public override bool CanEqual(object obj)
            {
                return obj is SubclassForAbstractRedefinablePoint;
            }

            public override bool Equals(object obj)
            {
                var other = obj as SubclassForAbstractRedefinablePoint;
                return other != null
                && other.CanEqual(this)
                && base.Equals(obj)
                && _color == other._color;
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }
    }
}

