using System;
using EqualsVerifier.TestHelpers;
using EqualsVerifier.TestHelpers.Types;
using NUnit.Framework;

namespace EqualsVerifier.Integration.ExtraFeatures
{
    [TestFixture]
    public class RelaxedEqualsTest : IntegrationTestBase
    {
        Multiple _a;
        Multiple _b;
        Multiple _x;

        [SetUp]
        public void Setup()
        {
            _a = new Multiple(1, 2);
            _b = new Multiple(2, 1);
            _x = new Multiple(2, 2);
        }

        [Test]
        public void WhenObjectsWithDifferentFieldsAreEqual_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForExamples(_a, _b).Verify(),
                "Precondition", 
                "two objects are equal to each other");
        }

        [Test]
        public void GivenTheyAreGivenAsRelaxedEqualExamples_WhenObjectsWithDifferentFieldsAreEqual_ThenSucceed()
        {
            EqualsVerifier
                .ForRelaxedEqualExamples(_a, _b)
                .AndUnequalExample(_x)
                .Verify();
        }

        [Test]
        public void WhenTheSameObjectIsGivenAsAnUnequalExample_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForRelaxedEqualExamples(_a, _b).AndUnequalExamples(_a).Verify(),
                "Precondition", 
                "the same object appears twice", 
                typeof(Multiple).Name);
        }

        [Test]
        public void GivenItIsGivenAsARelaxedEqualExample_WhenAnUnusedFieldIsNull_ThenSucceed()
        {
            EqualsVerifier
                .ForRelaxedEqualExamples(new NullContainingSubMultiple(1, 2), new NullContainingSubMultiple(2, 1))
                .AndUnequalExample(new NullContainingSubMultiple(2, 2))
                .Verify();
        }

        public class NullContainingSubMultiple : Multiple
        {
            readonly string _noValue = null;

            public NullContainingSubMultiple(int a, int b) : base(a, b)
            {
            }
        }
    }
}

