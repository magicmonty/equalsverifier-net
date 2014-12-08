using System;
using EqualsVerifier.TestHelpers;
using NUnit.Framework;
using EqualsVerifier.TestHelpers.Types;
using EqualsVerifier.Util.Exceptions;

namespace EqualsVerifier.Integration.ExtendedContract
{
    [TestFixture]
    public class NullFieldsTest : IntegrationTestBase
    {
        const string NON_NULLITY = "Non-nullity";
        const string EQUALS = "Equals throws NullReferenceException";
        const string HASHCODE = "hashCode throws NullReferenceException";
        const string ON_FIELD = "on field";

        [Test]
        public void WhenEqualsThrowsNullReferenceExceptionOnThisField_ThenFail()
        {
            ExpectFailureWithCause<NullReferenceException>(
                () => EqualsVerifier.ForType<EqualsThrowsNullReferenceExceptionOnThis>().Verify(),
                NON_NULLITY, 
                EQUALS, 
                ON_FIELD, 
                "_color");
        }

        [Test]
        public void WhenEqualsThrowsNullReferenceExceptioneOnOthersField_ThenFail()
        {
            ExpectFailureWithCause<NullReferenceException>(
                () => EqualsVerifier.ForType<EqualsThrowsNullReferenceExceptionOnOther>().Verify(),
                NON_NULLITY, 
                EQUALS, 
                ON_FIELD, 
                "_color");
        }

        [Test]
        public void WhenHashCodeThrowsNre_ThenFail()
        {
            ExpectFailureWithCause<NullReferenceException>(
                () => EqualsVerifier.ForType<HashCodeThrowsNullReferenceException>().Verify(),
                NON_NULLITY, 
                HASHCODE, 
                ON_FIELD, 
                "_color");
        }

        [Test]
        public void GivenExamples_WhenEqualsThrowsNpeOnThissField_ThenSucceed()
        {
            var blue = new EqualsThrowsNullReferenceExceptionOnThis(Color.BLUE);
            var yellow = new EqualsThrowsNullReferenceExceptionOnThis(Color.YELLOW);

            EqualsVerifier
                .ForExamples(blue, yellow)
                .Suppress(Warning.NULL_FIELDS)
                .Verify();
        }

        [Test]
        public void GivenWarningIsSuppressed_WhenEqualsThrowsNpeOnThissField_ThenSucceed()
        {
            EqualsVerifier
                .ForType<EqualsThrowsNullReferenceExceptionOnThis>()
                .Suppress(Warning.NULL_FIELDS)
                .Verify();
        }

        [Test]
        public void WhenEqualsTestFieldWhichThrowsNullReferenceException_ThenSucceed()
        {
            EqualsVerifier
                .ForType<CheckedDeepNullA>()
                .Verify();
        }

        [Test]
        public void GivenWarningIsSuppressed_WhenEqualsThrowsNpeOnFieldWhichAlsoThrowsNpe_ThenSucceed()
        {
            EqualsVerifier
                .ForType<DeepNullA>()
                .Suppress(Warning.NULL_FIELDS)
                .Verify();
        }

        [Test]
        public void GivenWarningIsSuppressed_WhenDoingASanityCheckOnTheFieldUsedInThePreviousTests_ThenSucceed()
        {
            EqualsVerifier
                .ForType<DeepNullB>()
                .Suppress(Warning.NULL_FIELDS)
                .Verify();
        }

        #pragma warning disable 414
        public sealed class EqualsThrowsNullReferenceExceptionOnThis
        {
            readonly object _color;

            public EqualsThrowsNullReferenceExceptionOnThis(Color color)
            {
                _color = color;
            }

            public override bool Equals(object obj)
            {
                var other = obj as EqualsThrowsNullReferenceExceptionOnThis;
                return other != null && _color.Equals(other._color);
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        public sealed class EqualsThrowsNullReferenceExceptionOnOther
        {
            readonly object _color;

            public EqualsThrowsNullReferenceExceptionOnOther(Color color)
            {
                _color = color;
            }

            public override bool Equals(object obj)
            {
                var other = obj as EqualsThrowsNullReferenceExceptionOnOther;
                return other != null
                && other._color.Equals(_color);
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        public sealed class HashCodeThrowsNullReferenceException
        {
            readonly object _color;

            public HashCodeThrowsNullReferenceException(Color color)
            {
                _color = color;
            }

            public override bool Equals(object obj)
            {
                return this.DefaultEquals(obj);
            }

            public override int GetHashCode()
            {
                return _color.GetHashCode();
            }

            public override string ToString()
            {
                return "";
            }
        }

        public sealed class CheckedDeepNullA
        {
            readonly DeepNullB _b;

            public CheckedDeepNullA(DeepNullB b)
            {
                _b = b;
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

        public sealed class DeepNullA
        {
            readonly DeepNullB _b;

            public DeepNullA(DeepNullB b)
            {
                if (b == null)
                    throw new NullReferenceException("b");

                _b = b;
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

        public sealed class DeepNullB
        {
            readonly Object _o;

            public DeepNullB(Object o)
            {
                if (o == null)
                    throw new NullReferenceException("o");

                _o = o;
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
    }
}

