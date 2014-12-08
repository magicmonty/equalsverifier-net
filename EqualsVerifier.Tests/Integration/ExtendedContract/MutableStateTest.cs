using NUnit.Framework;
using EqualsVerifier.TestHelpers;
using EqualsVerifier.Util;

namespace EqualsVerifier.Integration.ExtendedContract
{
    [TestFixture]
    public class MutableStateTest : IntegrationTestBase
    {
        const string MUTABILITY = "Mutability: equals depends on mutable field";
        const string FIELD_NAME = "_field";

        [Test]
        public void WhenClassHasAMutablePrimitiveField_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<PrimitiveMutableField>().Verify(),
                MUTABILITY, 
                "_second");
        }

        [Test]
        public void GivenItDoesNotUseThatFieldInEquals_WhenClassHasAMutablePrimitiveField_ThenSucceed()
        {
            EqualsVerifier
                .ForType<UnusedPrimitiveMutableField>()
                .Verify();
        }

        [Test]
        public void GivenWarningIsSuppressed_WhenClassHasAMutablePrimitiveField_ThenSucceed()
        {
            EqualsVerifier
                .ForType<PrimitiveMutableField>()
                .Suppress(Warning.NONFINAL_FIELDS)
                .Verify();
        }

        [Test]
        public void WhenClassHasAMutableObjectField_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<ObjectMutableField>().Verify(),
                MUTABILITY, 
                FIELD_NAME);
        }

        [Test]
        public void GivenWarningIsSuppressed_WhenClassHasAMutableObjectField_ThenSucceed()
        {
            EqualsVerifier
                .ForType<ObjectMutableField>()
                .Suppress(Warning.NONFINAL_FIELDS)
                .Verify();
        }

        [Test]
        public void WhenClassHasAMutableEnumField_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<EnumMutableField>().Verify(),
                MUTABILITY, 
                FIELD_NAME);
        }

        [Test]
        public void GivenWarningIsSuppressed_WhenClassHasAMutableEnumField_ThenSucceed()
        {
            EqualsVerifier
                .ForType<EnumMutableField>()
                .Suppress(Warning.NONFINAL_FIELDS)
                .Verify();
        }

        [Test]
        public void GivenUNKNOWNAsAnExample_WhenClassHasAMutableEnumField_ThenFail()
        {
            var red = new EnumMutableField(EnumMutableField.Enum.UNKOWN);
            var black = new EnumMutableField(EnumMutableField.Enum.BLACK);

            ExpectFailure(
                () => EqualsVerifier.ForExamples(red, black).Verify(),
                MUTABILITY, 
                FIELD_NAME);
        }

        [Test]
        public void GivenUNKNOWNAsAnExampleAndWarningIsSuppressed_WhenClassHasAMutableEnumField_ThenSucceed()
        {
            var red = new EnumMutableField(EnumMutableField.Enum.UNKOWN);
            var black = new EnumMutableField(EnumMutableField.Enum.BLACK);

            EqualsVerifier
                .ForExamples(red, black)
                .Suppress(Warning.NONFINAL_FIELDS)
                .Verify();
        }

        [Test]
        public void WhenClassHasAMutableArrayField_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<ArrayMutableField>().Verify(),
                MUTABILITY, 
                FIELD_NAME);
        }

        [Test]
        public void GivenWarningIsSuppressed_WhenClassHasAMutableArrayField_ThenSucceed()
        {
            EqualsVerifier
                .ForType<ArrayMutableField>()
                .Suppress(Warning.NONFINAL_FIELDS)
                .Verify();
        }

        #pragma warning disable 414
        public sealed class PrimitiveMutableField
        {
            int _second;

            public PrimitiveMutableField(int second)
            {
                _second = second;
            }

            public override bool Equals(object obj)
            {
                var other = obj as PrimitiveMutableField;
                return other != null
                && _second == other._second;
            }

            public override int GetHashCode()
            {
                return _second;
            }
        }

        public sealed class UnusedPrimitiveMutableField
        {
            readonly int _immutable;
            int _mutable = 0;

            public UnusedPrimitiveMutableField(int value)
            {
                _immutable = value;
            }

            public override bool Equals(object obj)
            {
                var other = obj as UnusedPrimitiveMutableField;
                return other != null && _immutable == other._immutable;
            }

            public override int GetHashCode()
            {
                return _immutable;
            }
        }

        public sealed class ObjectMutableField
        {
            object _field;

            public ObjectMutableField(object value)
            {
                _field = value;
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

        public sealed class EnumMutableField
        {
            public enum Enum
            {
                RED,
                BLACK,
                UNKOWN
            }

            Enum _field;

            public EnumMutableField(Enum value)
            {
                _field = value;
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

        public sealed class ArrayMutableField
        {
            int[] _field;

            public ArrayMutableField(int[] value)
            {
                _field = value;
            }

            public override bool Equals(object obj)
            {
                var other = obj as ArrayMutableField;
                return other != null
                && _field.ArrayDeeplyEquals(other._field);
            }

            public override int GetHashCode()
            {
                return (_field == null) ? 0 : _field.GetDeepHashCode();
            }
        }
        #pragma warning restore 414
    }
}

