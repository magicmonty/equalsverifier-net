using System;
using EqualsVerifier.TestHelpers;
using NUnit.Framework;
using EqualsVerifier.TestHelpers.Types;

namespace EqualsVerifier.Integration.ExtraFeatures
{
    [TestFixture]
    public class RelaxedEqualsPreconditionTest : IntegrationTestBase
    {
        const string PRECONDITION = "Precondition";
        const string DIFFERENT_CLASSES = "are of different classes";
        const string TWO_IDENTICAL_OBJECTS_APPEAR = "two identical objects appear";
        const string NOT_ALL_EQUAL = "not all equal objects are equal";
        const string OBJECT_APPEARS_TWICE = "the same object appears twice";
        const string TWO_OBJECTS_ARE_EQUAL = "two objects are equal to each other";

        Multiple _red;
        Multiple _black;
        Multiple _green;

        [SetUp]
        public void Setup()
        {
            _red = new Multiple(1, 2);
            _black = new Multiple(2, 1);
            _green = new Multiple(2, 2);
        }

        [Test]
        public void WhenTheFirstExampleIsNull_ThenThrowArgumentException()
        {
            ExpectException<ArgumentNullException>(
                () => EqualsVerifier.ForRelaxedEqualExamples(null, _black),
                "Argument cannot be null.",
                "Parameter name: first");
        }

        [Test]
        public void WhenTheSecondExampleIsNull_ThenThrowArgumentException()
        {
            ExpectException<ArgumentNullException>(
                () => EqualsVerifier.ForRelaxedEqualExamples(_red, null),
                "Argument cannot be null.",
                "Parameter name: second");
        }

        [Test]
        public void WhenTheVarargArrayIsNull_ThenSucceed()
        {
            EqualsVerifier
                .ForRelaxedEqualExamples(_red, _black, (Multiple[])null)
                .AndUnequalExample(_green)
                .Verify();
        }

        [Test]
        public void WhenAVarargParameterIsNull_ThenFail()
        {
            var another = new Multiple(-1, -2);
            ExpectException<ArgumentException>(
                () => EqualsVerifier.ForRelaxedEqualExamples(_red, _black, another, null),
                "One of the examples is null.");
        }

        [Test]
        public void WhenTheUnequalExampleIsNull_ThenFail()
        {
            ExpectException<ArgumentNullException>(
                () => EqualsVerifier.ForRelaxedEqualExamples(_red, _black).AndUnequalExample(null),
                "Argument cannot be null.",
                "Parameter name: first");
        }

        [Test]
        public void WhenTheUnequalVarargArrayIsNull_ThenSucceed()
        {
            EqualsVerifier
                .ForRelaxedEqualExamples(_red, _black)
                .AndUnequalExamples(_green, (Multiple[])null)
                .Verify();
        }

        [Test]
        public void WhenAnUnequalVarargParameterIsNull_ThenFail()
        {
            var another = new Multiple(3, 3);
            ExpectException<ArgumentException>(
                () => EqualsVerifier.ForRelaxedEqualExamples(_red, _black).AndUnequalExamples(_green, another, null)
            );
        }

        [Test]
        public void WhenEqualExamplesAreOfDifferentRuntimeTypes_ThenFail()
        {
            var sm = new SubMultiple(1, 2);
            ExpectFailure(
                () => EqualsVerifier.ForRelaxedEqualExamples(sm, _red).AndUnequalExample(_green).Verify(),
                PRECONDITION, 
                DIFFERENT_CLASSES, 
                typeof(SubMultiple).Name, 
                typeof(Multiple).Name);
        }

        [Test]
        public void WhenTheSameExampleIsGivenTwice_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForRelaxedEqualExamples(_red, _red).AndUnequalExample(_green).Verify(),
                PRECONDITION, 
                OBJECT_APPEARS_TWICE, 
                typeof(Multiple).Name);
        }

        [Test]
        public void WhenTwoExamplesAreEqual_ThenFail()
        {
            var aa = new Multiple(1, 2);
            ExpectFailure(
                () => EqualsVerifier.ForRelaxedEqualExamples(_red, aa).AndUnequalExample(_green).Verify(),
                PRECONDITION, 
                TWO_IDENTICAL_OBJECTS_APPEAR, 
                typeof(Multiple).Name);
        }

        [Test]
        public void WhenAnEqualExampleIsAlsoGivenAsAnUnequalExample_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForRelaxedEqualExamples(_red, _green).AndUnequalExample(_green).Verify(),
                PRECONDITION, 
                NOT_ALL_EQUAL, 
                "and", 
                typeof(Multiple).Name);
        }

        [Test]
        public void WhenTheSameUnequalExampleIsGivenTwice_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForRelaxedEqualExamples(_red, _black).AndUnequalExamples(_green, _green).Verify(),
                PRECONDITION, 
                OBJECT_APPEARS_TWICE, 
                typeof(Multiple).Name);
        }

        [Test]
        public void WhenTwoUnequalExamplesAreEqualToEachOther_ThenFail()
        {
            var xx = new Multiple(2, 2);
            ExpectFailure(
                () => EqualsVerifier.ForRelaxedEqualExamples(_red, _black).AndUnequalExamples(_green, xx).Verify(),
                PRECONDITION, 
                TWO_OBJECTS_ARE_EQUAL, 
                typeof(Multiple).Name);
        }

        public class SubMultiple : Multiple
        {
            public SubMultiple(int a, int b) : base(a, b)
            {
            }
        }
    }
}

