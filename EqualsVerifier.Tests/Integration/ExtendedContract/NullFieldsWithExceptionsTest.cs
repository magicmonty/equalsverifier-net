using System;
using EqualsVerifier.TestHelpers;
using NUnit.Framework;

namespace EqualsVerifier.Integration.ExtendedContract
{
    [TestFixture]
    public class NullFieldsWithExceptionsTest : IntegrationTestBase
    {
        const string EQUALS = "Equals";
        const string GET_HASH_CODE = "GetHashCode";
        const string THROWS = "throws";
        const string SYSTEM_EXCEPTION = "SystemException";
        const string INVALID_OPERATION_EXCEPTION = "InvalidOperationException";
        const string WHEN_FOO_IS_NULL = "when field _foo is null";

        [Test]
        public void GivenFieldIsNull_WhenSystemExceptionIsThrownInEquals_RecogniseUnderlyingException()
        {
            ExpectFailureWithCause<SystemException>(
                () => EqualsVerifier.ForType<EqualsSystemExceptionThrower>().Verify(),
                EQUALS, 
                THROWS, 
                SYSTEM_EXCEPTION, 
                WHEN_FOO_IS_NULL);
        }

        [Test]
        public void GivenFieldIsNull_WhenInvalidOperationExceptionIsThrownInEquals_RecogniseUnderlyingException()
        {
            ExpectFailureWithCause<InvalidOperationException>(
                () => EqualsVerifier.ForType<EqualsInvalidOperationExceptionThrower>().Verify(),
                EQUALS, 
                THROWS, 
                INVALID_OPERATION_EXCEPTION, 
                WHEN_FOO_IS_NULL);
        }

        [Test]
        public void GivenFieldIsNull_WhenSystemExceptionIsThrownInHashCode_RecogniseUnderlyingException()
        {
            ExpectFailureWithCause<SystemException>(
                () => EqualsVerifier.ForType<GetHashCodeSystemExceptionThrower>().Verify(),
                GET_HASH_CODE, 
                THROWS, 
                SYSTEM_EXCEPTION, 
                WHEN_FOO_IS_NULL);
        }

        [Test]
        public void GivenFieldIsNull_WhenInvalidOperationExceptionIsThrownInHashCode_RecogniseUnderlyingException()
        {
            ExpectFailureWithCause<InvalidOperationException>(
                () => EqualsVerifier.ForType<GetHashCodeInvalidOperationExceptionThrower>().Verify(),
                GET_HASH_CODE, 
                THROWS, 
                INVALID_OPERATION_EXCEPTION, 
                WHEN_FOO_IS_NULL);
        }

        public abstract class EqualsThrower
        {
            readonly string _foo;

            protected abstract Exception ThrowException();

            public EqualsThrower(string foo)
            {
                _foo = foo;
            }

            public sealed override bool Equals(object obj)
            {
                var other = obj as EqualsThrower;
                if (other == null)
                    return false;

                if (_foo == null)
                    throw ThrowException();

                return _foo.NullSafeEquals(other._foo);
            }

            public sealed override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        public class EqualsSystemExceptionThrower : EqualsThrower
        {
            public EqualsSystemExceptionThrower(string foo) : base(foo)
            {
            }

            protected override Exception ThrowException()
            {
                return new SystemException();
            }
        }

        public class EqualsInvalidOperationExceptionThrower : EqualsThrower
        {
            public EqualsInvalidOperationExceptionThrower(string foo) : base(foo)
            {
            }

            protected override Exception ThrowException()
            {
                return new InvalidOperationException();
            }
        }

        public abstract class GetHashCodeThrower
        {
            readonly string _foo;

            protected abstract Exception ThrowException();

            public GetHashCodeThrower(string foo)
            {
                _foo = foo;
            }

            public sealed override bool Equals(object obj)
            {
                return this.DefaultEquals(obj);
            }

            public sealed override int GetHashCode()
            {
                if (_foo == null)
                    throw ThrowException();

                return _foo.GetHashCode();
            }
        }

        public class GetHashCodeSystemExceptionThrower : GetHashCodeThrower
        {
            public GetHashCodeSystemExceptionThrower(string foo) : base(foo)
            {
            }

            protected override Exception ThrowException()
            {
                return new SystemException();
            }
        }

        public class GetHashCodeInvalidOperationExceptionThrower : GetHashCodeThrower
        {
            public GetHashCodeInvalidOperationExceptionThrower(string foo) : base(foo)
            {
            }

            protected override Exception ThrowException()
            {
                return new InvalidOperationException();
            }
        }
    }
}

