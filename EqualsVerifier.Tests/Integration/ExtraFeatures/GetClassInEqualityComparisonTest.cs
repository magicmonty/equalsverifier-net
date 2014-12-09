using NUnit.Framework;
using EqualsVerifier.TestHelpers;

namespace EqualsVerifier.Integration.ExtraFeatures
{
    [TestFixture]
    public class GetClassInEqualityComparisonTest : IntegrationTestBase
    {
        [Test]
        public void GivenAnAbstractSuperclassAndUsingGetClassIsUsed_WhenGetClassIsPartOfEqualityComparison_ThenSucceed()
        {
            EqualsVerifier
                .ForType<Identifiable>()
                .UsingGetType()
                .Suppress(Warning.IDENTICAL_COPY) // Needed, because there are different proxy types generated
                .Verify();
        }

        [Test]
        public void GivenAConcreteImplementationAndUsingGetClassIsUsed_WhenGetClassIsPartOfEqualityComparison_ThenSucceed()
        {
            EqualsVerifier
                .ForType<Person>()
                .UsingGetType()
                .Verify();
        }

        [Test]
        public void GivenAnotherConcreteImplementationAndUsingGetClassIsUsed_WhenGetClassIsPartOfEqualityComparison_ThenSucceed()
        {
            EqualsVerifier
                .ForType<Account>()
                .UsingGetType()
                .Verify();
        }

        public abstract class Identifiable
        {
            readonly int _id;

            public Identifiable(int id)
            {
                _id = id;
            }

            public sealed override bool Equals(object obj)
            {
                var other = obj as Identifiable;
                return other != null
                && _id == other._id
                && GetType() == other.GetType();
            }

            public sealed override int GetHashCode()
            {
                return _id;
            }
        }

        public class Person : Identifiable
        {
            public Person(int id) : base(id)
            {
            }
        }

        public class Account : Identifiable
        {
            public Account(int id) : base(id)
            {
            }
        }
    }
}

