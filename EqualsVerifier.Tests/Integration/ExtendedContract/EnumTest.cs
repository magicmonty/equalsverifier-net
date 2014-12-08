using System;
using NUnit.Framework;
using EqualsVerifier.TestHelpers.Types;
using EqualsVerifier.TestHelpers;

namespace EqualsVerifier.Integration.ExtendedContract
{
    [TestFixture]
    public class EnumTest
    {
        [Test]
        public void WhenClassIsAnEnum_ThenItCannotBeVerified()
        {
            // Would not work, as the type parameter must be a class type currently
            // EqualsVerifier.ForType<Enum>().Verify();
        }

        [Test]
        public void IgnoreSingleValueEnum()
        {
            EqualsVerifier.ForType<SingletonContainer>().Verify();
        }

        [Test]
        public void UseSingleValueEnum()
        {
            EqualsVerifier.ForType<SingletonUser>().Verify();
        }

        #pragma warning disable 414
        public enum Enum
        {
            ONE,
            TWO,
            THREE
        }

        public enum Singleton
        {
            INSTANCE
        }

        public sealed class SingletonContainer
        {
            readonly int _i;

            readonly Singleton _singleton;

            public SingletonContainer(int i)
            {
                _i = i;
                _singleton = Singleton.INSTANCE;
            }

            public override bool Equals(object obj)
            {
                var other = obj as SingletonContainer;
                return other != null && _i == other._i;
            }

            public override int GetHashCode()
            {
                return _i;
            }
        }

        public sealed class SingletonUser
        {
            readonly Singleton _singleton;

            public SingletonUser(Singleton singleton)
            {
                _singleton = singleton;
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

