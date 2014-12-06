using EqualsVerifier.TestHelpers;
using NUnit.Framework;

namespace EqualsVerifier.Integration.ExtendedContract
{
    [TestFixture]
    public class BalancedAbstractnessTest : IntegrationTestBase
    {
        const string ABSTRACT_DELEGATION = "Abstract delegation";
        const string BOTH_ARE_ABSTRACT = "Equals and GetHashCode methods are both abstract";
        const string Equals_IS_ABSTRACT = "Equals method is abstract";
        const string HASHCODE_IS_ABSTRACT = "GetHashCode method is abstract";
        const string Equals_IS_NOT = "but Equals is not";
        const string HASHCODE_IS_NOT = "but GetHashCode is not";

        [Test]
        public void WhenBothEqualsAndHashCodeAreAbstract_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<AbstractBoth>().Verify(),
                BOTH_ARE_ABSTRACT, 
                typeof(AbstractBoth).Name);
        }

        [Test]
        public void WhenEqualsIsAbstract_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<AbstractEquals>().Verify(),
                Equals_IS_ABSTRACT, 
                HASHCODE_IS_NOT, 
                typeof(AbstractEquals).Name);
        }

        [Test]
        public void WhenHashCodeIsAbstract_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<AbstractHashCode>().Verify(),
                HASHCODE_IS_ABSTRACT, 
                Equals_IS_NOT, 
                typeof(AbstractHashCode).Name);
        }

        [Test]
        public void WhenBothAreAbstractInSuperclass_ThenSucceed()
        {
            EqualsVerifier
                .ForType<SubclassOfAbstractBoth>()
                .Verify();
        }

        [Test]
        public void WhenOnlyEqualsIsAbstractInSuperclass_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<SubclassOfAbstractEqualsButNotHashCode>().Verify(),
                ABSTRACT_DELEGATION, 
                Equals_IS_ABSTRACT, 
                HASHCODE_IS_NOT, 
                typeof(AbstractEqualsButNotHashCode).Name);
        }

        [Test]
        public void WhenOnlyHashCodeIsAbstractInSuperclass_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<SubclassOfAbstractHashCodeButNotEquals>().Verify(),
                ABSTRACT_DELEGATION, 
                HASHCODE_IS_ABSTRACT, 
                Equals_IS_NOT, 
                typeof(AbstractHashCodeButNotEquals).Name);
        }

        [Test]
        public void WhenBothAreAbstractInSuperclassOfSuperclass_ThenSucceed()
        {
            EqualsVerifier
                .ForType<SubclassOfSubclassOfAbstractBoth>()
                .Verify();
        }

        #pragma warning disable 414
        #pragma warning disable 659
        public abstract class AbstractBoth
        {
            public override abstract bool Equals(object obj);

            public override abstract int GetHashCode();
        }

        public abstract class AbstractEquals
        {
            int _i;

            public AbstractEquals(int i)
            {
                _i = i;
            }

            public override abstract bool Equals(object obj);

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        public abstract class AbstractHashCode
        {
            int _i;

            public AbstractHashCode(int i)
            {
                _i = i;
            }

            public override bool Equals(object obj)
            {
                return this.DefaultEquals(obj);
            }

            public override abstract int GetHashCode();
        }

        public sealed class SubclassOfAbstractBoth : AbstractBoth
        {
            readonly int _foo;

            public SubclassOfAbstractBoth(int foo)
            {
                _foo = foo;
            }

            public override bool Equals(object obj)
            {
                return this.DefaultEquals(obj);
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        public abstract class AbstractEqualsButNotHashCode
        {
            public override abstract bool Equals(object obj);
        }

        public class SubclassOfAbstractEqualsButNotHashCode : AbstractEqualsButNotHashCode
        {
            readonly int _foo;

            public SubclassOfAbstractEqualsButNotHashCode(int foo)
            {
                _foo = foo;
            }

            public override bool Equals(object obj)
            {
                return this.DefaultEquals(obj);
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        public abstract class AbstractHashCodeButNotEquals
        {
            public override abstract int GetHashCode();
        }

        public class SubclassOfAbstractHashCodeButNotEquals : AbstractHashCodeButNotEquals
        {
            readonly int _foo;

            public SubclassOfAbstractHashCodeButNotEquals(int foo)
            {
                _foo = foo;
            }

            public override bool Equals(object obj)
            {
                return this.DefaultEquals(obj);
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        public abstract class IntermediateSubclassOfAbstractBoth : AbstractBoth
        {

        }

        public sealed class SubclassOfSubclassOfAbstractBoth : IntermediateSubclassOfAbstractBoth
        {
            readonly int _foo;

            public SubclassOfSubclassOfAbstractBoth(int foo)
            {
                _foo = foo;
            }

            public override bool Equals(object obj)
            {
                return this.DefaultEquals(obj);
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }
        #pragma warning restore 414
        #pragma warning restore 659
    }
}

