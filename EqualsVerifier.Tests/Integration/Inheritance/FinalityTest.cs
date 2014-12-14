using System;
using EqualsVerifier.TestHelpers;
using NUnit.Framework;
using EqualsVerifier.TestHelpers.Types;

namespace EqualsVerifier.Integration.Inheritance
{
    [TestFixture]
    public class FinalityTest : IntegrationTestBase
    {
        const string BOTH_FINAL_OR_NONFINAL = "Finality: Equals and GetHashCode must both be sealed or both be non-sealed";
        const string SUBCLASS = "Subclass";
        const string SUPPLY_AN_INSTANCE = "Supply an instance of a redefined subclass using WithRedefinedSubclass";

        [Test]
        public void WhenEqualsIsSealedButGetHashCodeIsNotSealed_ThenFail()
        {
            Check<SealedEqualsNonSealedGetHashCode>();
        }

        [Test]
        public void GivenAClassThatIsNotSealed_WhenEqualsIsNotSealed()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<Point>().Verify(),
                SUBCLASS, 
                "Equals is not sealed", 
                SUPPLY_AN_INSTANCE, 
                "if Equals cannot be sealed");
        }

        [Test]
        public void WhenEqualsIsSealedButGetHashCodeIsNonSealed_ThenSucceed()
        {
            EqualsVerifier
                .ForType<SealedEqualsNonSealedGetHashCode>()
                .UsingGetType()
                .Suppress(Warning.STRICT_INHERITANCE)
                .Verify();
        }

        [Test]
        public void GivenAClassThatIsNotSealedAndWarningIsSuppressed_WhenEqualsIsNotSealed_ThenSucceed()
        {
            EqualsVerifier
                .ForType<Point>()
                .Suppress(Warning.STRICT_INHERITANCE)
                .Verify();
        }

        [Test]
        public void WhenEqualsIsNotSealedButHashCodeIsSealed_ThenFail()
        {
            Check<NonSealedEqualsSealedGetHashCode>();
        }

        [Test]
        public void WhenGetHashCodeIsNotSealed_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<FinalEqualsPoint>().Verify(),
                SUBCLASS, 
                "GetHashCode is not sealed", 
                SUPPLY_AN_INSTANCE, 
                "if GetHashCode cannot be sealed");
        }

        [Test]
        public void GivenWarningsAreSuppressed_WhenEqualsIsNotSealedButGetHashCodeIsSealed_ThenSucceed()
        {
            EqualsVerifier
                .ForType<NonSealedEqualsSealedGetHashCode>()
                .UsingGetType()
                .Suppress(Warning.STRICT_INHERITANCE)
                .Verify();
        }

        [Test]
        public void GivenAClassThatIsNotSealedAndAnEqualsMethodThatIsSealedAndWarningIsSuppressed_WhenHashCodeIsNotFinal_ThenSucceed()
        {
            EqualsVerifier
                .ForType<FinalEqualsPoint>()
                .Suppress(Warning.STRICT_INHERITANCE)
                .Verify();
        }

        private void Check<T>() where T:class
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<T>().UsingGetType().Verify(),
                BOTH_FINAL_OR_NONFINAL);
        }

        public class SealedEqualsNonSealedGetHashCode
        {
            readonly int _i;

            public SealedEqualsNonSealedGetHashCode(int i)
            {
                _i = i;
            }

            public override sealed bool Equals(object obj)
            {
                if (obj == null || obj.GetType() != GetType())
                    return false;
                var other = (SealedEqualsNonSealedGetHashCode)obj;
                return other._i == _i;
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        public class NonSealedEqualsSealedGetHashCode
        {
            readonly int _i;

            public NonSealedEqualsSealedGetHashCode(int i)
            {
                _i = i;
            }

            public override bool Equals(object obj)
            {
                if (obj == null || obj.GetType() != GetType())
                    return false;
                var other = (NonSealedEqualsSealedGetHashCode)obj;
                return other._i == _i;
            }

     
            public override sealed int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        public class FinalEqualsPoint : Point
        {
            public FinalEqualsPoint(int x, int y) : base(x, y)
            {
            }

            public override sealed bool Equals(Object obj)
            {
                return base.Equals(obj);
            }
        }
    }
}