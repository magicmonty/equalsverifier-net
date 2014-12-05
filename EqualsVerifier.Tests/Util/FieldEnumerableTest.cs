using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;
using Shouldly;
using System;
using EqualsVerifier.TestHelpers.Types;

namespace EqualsVerifier.Util
{
    [TestFixture]
    public class FieldEnumerableTest
    {
        [Test]
        public void SimpleFields()
        {
            var actual = new HashSet<FieldInfo>();
            foreach (var field in FieldEnumerable.Of(typeof(TypeHelper.DifferentAccessModifiersFieldContainer)))
            {
                actual.Add(field);
            }

            FIELD_CONTAINER_FIELDS.ShouldBeSubsetOf(actual);
            actual.ShouldBeSubsetOf(FIELD_CONTAINER_FIELDS);
        }

        [Test]
        public void SubAndSuperClassFields()
        {
            var actual = new HashSet<FieldInfo>();
            foreach (var field in FieldEnumerable.Of(typeof(TypeHelper.DifferentAccessModifiersSubFieldContainer)))
            {
                actual.Add(field);
            }

            FIELD_AND_SUB_FIELD_CONTAINER_FIELDS.ShouldBeSubsetOf(actual);
            actual.ShouldBeSubsetOf(FIELD_AND_SUB_FIELD_CONTAINER_FIELDS);
        }

        [Test]
        public void OnlySubClassFields()
        {
            var actual = new HashSet<FieldInfo>();
            foreach (var field in FieldEnumerable.OfIgnoringSuper(typeof(TypeHelper.DifferentAccessModifiersSubFieldContainer)))
                actual.Add(field);

            SUB_FIELD_CONTAINER_FIELDS.ShouldBeSubsetOf(actual);
            actual.ShouldBeSubsetOf(SUB_FIELD_CONTAINER_FIELDS);
        }

        [Test]
        public void NoFields()
        {
            var iterable = FieldEnumerable.Of(typeof(TypeHelper.NoFields));
            iterable.ShouldBeEmpty();
        }

        [Test]
        public void SuperHasNoFields()
        {
            var expected = new HashSet<FieldInfo>();
            expected.Add(typeof(TypeHelper.NoFieldsSubWithFields).GetField("Field"));

            var actual = new HashSet<FieldInfo>();
            foreach (var field in FieldEnumerable.Of(typeof(TypeHelper.NoFieldsSubWithFields)))
                actual.Add(field);

            actual.ShouldBeSubsetOf(expected);
            expected.ShouldBeSubsetOf(actual);
        }

        [Test]
        public void SubHasNoFields()
        {
            var actual = new HashSet<FieldInfo>();
            foreach (var field in FieldEnumerable.Of(typeof(TypeHelper.EmptySubFieldContainer)))
                actual.Add(field);

            actual.ShouldBeSubsetOf(FIELD_CONTAINER_FIELDS);
            FIELD_CONTAINER_FIELDS.ShouldBeSubsetOf(actual);
        }

        [Test]
        public void ClassInTheMiddleHasNoFields()
        {
            var expected = new HashSet<FieldInfo>(FieldContainerFields());
            expected.Add(typeof(TypeHelper.SubEmptySubFieldContainer).GetField("field"));

            var actual = new HashSet<FieldInfo>();
            foreach (var field in FieldEnumerable.Of(typeof(TypeHelper.SubEmptySubFieldContainer)))
            {
                actual.Add(field);
            }

            actual.ShouldBeSubsetOf(expected);
            expected.ShouldBeSubsetOf(actual);
        }

        [Test]
        public void InterfaceTest()
        {
            var iterable = FieldEnumerable.Of(typeof(TypeHelper.IInterface));
            iterable.ShouldBeEmpty();
        }

        [Test]
        public void ObjectHasNoElements()
        {
            var iterable = FieldEnumerable.Of(typeof(object));
            iterable.ShouldBeEmpty();
        }

        static readonly ISet<FieldInfo> FIELD_CONTAINER_FIELDS = new HashSet<FieldInfo>(FieldContainerFields());

        static IEnumerable<FieldInfo> FieldContainerFields()
        {
            var type = typeof(TypeHelper.DifferentAccessModifiersFieldContainer);
            yield return type.GetDeclaredField("i");
            yield return type.GetDeclaredField("j");
            yield return type.GetDeclaredField("k");
            yield return type.GetDeclaredField("l");
            yield return type.GetDeclaredField("I");
            yield return type.GetDeclaredField("J");
            yield return type.GetDeclaredField("K");
            yield return type.GetDeclaredField("L");
        }


        static readonly ISet<FieldInfo> SUB_FIELD_CONTAINER_FIELDS = new HashSet<FieldInfo>(SubFieldContainerFields());

        static IEnumerable<FieldInfo> SubFieldContainerFields()
        {
            var type = typeof(TypeHelper.DifferentAccessModifiersSubFieldContainer);
            yield return type.GetDeclaredField("a");
            yield return type.GetDeclaredField("b");
            yield return type.GetDeclaredField("c");
            yield return type.GetDeclaredField("d");
        }

        static readonly ISet<FieldInfo> FIELD_AND_SUB_FIELD_CONTAINER_FIELDS = new HashSet<FieldInfo>(FieldAndSubFieldContainerFields());

        static IEnumerable<FieldInfo> FieldAndSubFieldContainerFields()
        {
            foreach (var field in FIELD_CONTAINER_FIELDS)
                yield return field;
            foreach (var field in SUB_FIELD_CONTAINER_FIELDS)
                yield return field;
        }
    }

    static class TypeExtensions
    {
        public static FieldInfo GetDeclaredField(this Type type, string fieldName)
        {
            return type.GetField(fieldName, FieldHelper.AllFields);
        }
    }
}