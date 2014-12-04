using System;
using EqualsVerifier.TestHelpers.Types;
using NUnit.Framework;
using Shouldly;
using System.Reflection;

namespace EqualsVerifier.Util
{
    public class FieldAccessorTest
    {
        static readonly Point RED_NEW_POINT = new Point(10, 20);
        static readonly Point BLACK_NEW_POINT = new Point(20, 10);
        static readonly String FIELD_NAME = "Field";

        PrefabValues _prefabValues;

        [SetUp]
        public void Setup()
        {
            _prefabValues = new PrefabValues(new StaticFieldValueStash());
            NetApiPrefabValues.AddTo(_prefabValues);
        }

        [Test]
        public void GetObject()
        {
            var foo = new TypeHelper.ObjectContainer();
            var fieldAccessor = GetAccessorFor(foo, FIELD_NAME);
            fieldAccessor.Object.ShouldBeSameAs(foo);
        }

        [Test]
        public void GetField()
        {
            var foo = new TypeHelper.ObjectContainer();
            var field = foo.GetType().GetField(FIELD_NAME);
            var fieldAccessor = new FieldAccessor(foo, field);
            fieldAccessor.Field.ShouldBeSameAs(field);
        }

        [Test]
        public void GetFieldType()
        {
            var foo = new TypeHelper.ObjectContainer();
            var fieldAccessor = GetAccessorFor(foo, FIELD_NAME);
            fieldAccessor.FieldType.ShouldBe(typeof(object));
        }

        [Test]
        public void GetFieldName()
        {
            var foo = new TypeHelper.ObjectContainer();
            var fieldAccessor = GetAccessorFor(foo, FIELD_NAME);
            fieldAccessor.FieldName.ShouldBe(FIELD_NAME);
        }

        [Test]
        public void IsNotPrimitive()
        {
            var foo = new TypeHelper.ObjectContainer();
            var fieldAccessor = GetAccessorFor(foo, FIELD_NAME);
            fieldAccessor.IsPrimitive.ShouldBe(false);
        }

        [Test]
        public void IsPrimitive()
        {
            var foo = new TypeHelper.PrimitiveContainer();
            var fieldAccessor = GetAccessorFor(foo, FIELD_NAME);
            fieldAccessor.IsPrimitive.ShouldBe(true);
        }

        [Test]
        public void IsNotReadonly()
        {
            var foo = new TypeHelper.ObjectContainer();
            var fieldAccessor = GetAccessorFor(foo, FIELD_NAME);
            fieldAccessor.IsReadonly.ShouldBe(false);
        }

        [Test]
        public void IsReadonly()
        {
            var foo = new TypeHelper.ReadonlyContainer();
            var fieldAccessor = GetAccessorFor(foo, FIELD_NAME);
            fieldAccessor.IsReadonly.ShouldBe(true);
        }

        [Test]
        public void IsNotStatic()
        {
            var foo = new TypeHelper.ObjectContainer();
            var fieldAccessor = GetAccessorFor(foo, FIELD_NAME);
            fieldAccessor.IsStatic.ShouldBe(false);
        }

        [Test]
        public void IsStatic()
        {
            var foo = new TypeHelper.StaticContainer();
            var fieldAccessor = GetAccessorFor(foo, FIELD_NAME);
            fieldAccessor.IsStatic.ShouldBe(true);
        }

        [Test]
        public void GetValuePrimitive()
        {
            var foo = new TypeHelper.PrimitiveContainer();
            foo.Field = 10;
            var value = GetValue(foo, FIELD_NAME);
            value.ShouldBe(10);
        }

        [Test]
        public void GetValueObject()
        {
            var obj = new object();
            var foo = new TypeHelper.ObjectContainer();
            foo.Field = obj;
            var value = GetValue(foo, FIELD_NAME);
            value.ShouldBe(obj);
        }

        [Test]
        public void GetPrivateValue()
        {
            var foo = new TypeHelper.PrivateObjectContainer();
            var value = GetValue(foo, FIELD_NAME);
            value.ShouldNotBe(null);
            value.ShouldBeOfType(typeof(object));
        }

        [Test]
        public void SetValuePrimitive()
        {
            var foo = new TypeHelper.PrimitiveContainer();
            SetField(foo, FIELD_NAME, 20);
            foo.Field.ShouldBe(20);
        }

        [Test]
        public void SetValueObject()
        {
            var obj = new object();
            var foo = new TypeHelper.ObjectContainer();
            SetField(foo, FIELD_NAME, obj);
            foo.Field.ShouldBe(obj);
        }

        [Test]
        public void DefaultFieldOnObjectSetsNull()
        {
            var foo = new TypeHelper.ObjectContainer();
            foo.Field = new object();
            DefaultField(foo, FIELD_NAME);
            foo.Field.ShouldBe(null);
        }

        [Test]
        public void DefaultFieldOnArraySetsNull()
        {
            var foo = new TypeHelper.AllTypesContainer();
            foo._array = new [] { 1, 2, 3 };
            DefaultField(foo, "_array");
            foo._array.ShouldBe(null);
        }

        [Test]
        public void DefaultFieldOnBooleanSetsFalse()
        {
            var foo = new TypeHelper.AllTypesContainer();
            foo._boolean = true;
            DefaultField(foo, "_boolean");
            foo._boolean.ShouldBe(false);
        }

        [Test]
        public void DefaultFieldOnByteSetsZero()
        {
            var foo = new TypeHelper.AllTypesContainer();
            foo._byte = 10;
            DefaultField(foo, "_byte");
            foo._byte.ShouldBe((byte)0);
        }

        [Test]
        public void DefaultFieldOnDoubleSetsZero()
        {
            var foo = new TypeHelper.AllTypesContainer();
            foo._double = 1.1;
            DefaultField(foo, "_double");
            foo._double.ShouldBe(0.0, 0.0000001);
        }

        [Test]
        public void DefaultFieldOnFloatSetsZero()
        {
            var foo = new TypeHelper.AllTypesContainer();
            foo._float = 1.1f;
            DefaultField(foo, "_float");
            foo._float.ShouldBe(0.0f, 0.0000001);
        }

        [Test]
        public void DefaultFieldOnCharSetsZero()
        {
            var foo = new TypeHelper.AllTypesContainer();
            foo._char = 'a';
            DefaultField(foo, "_char");
            foo._char.ShouldBe('\u0000');
        }

        [Test]
        public void DefaultFieldOnIntSetsZero()
        {
            var foo = new TypeHelper.AllTypesContainer();
            foo._int = 10;
            DefaultField(foo, "_int");
            foo._int.ShouldBe(0);
        }

        [Test]
        public void DefaultFieldOnLongSetsZero()
        {
            var foo = new TypeHelper.AllTypesContainer();
            foo._long = 10;
            DefaultField(foo, "_long");
            foo._long.ShouldBe(0);
        }

        [Test]
        public void DefaultFieldOnShortSetsZero()
        {
            var foo = new TypeHelper.AllTypesContainer();
            foo._short = 10;
            DefaultField(foo, "_short");
            foo._short.ShouldBe((Int16)0);
        }

        [Test]
        public void DefaultFieldOnPrimitiveStaticReadonlyIsNoOp()
        {
            var foo = new TypeHelper.StaticFinalContainer();
            DefaultField(foo, "CONST");
            TypeHelper.StaticFinalContainer.CONST.ShouldBe(42);
        }

        [Test]
        public void DefaultFieldOnObjectStaticFinalIsNoOp()
        {
            var foo = new TypeHelper.StaticFinalContainer();
            var original = TypeHelper.StaticFinalContainer.OBJECT;
            DefaultField(foo, "OBJECT");
            TypeHelper.StaticFinalContainer.OBJECT.ShouldBeSameAs(original);
        }

        [Test]
        public void DefaultPrivateField()
        {
            var foo = new TypeHelper.PrivateObjectContainer();
            DefaultField(foo, FIELD_NAME);
            foo.Get().ShouldBe(null);
        }

        [Test]
        public void CopyToPrimitiveField()
        {
            var value = 10;

            var from = new TypeHelper.PrimitiveContainer();
            from.Field = value;

            var to = new TypeHelper.PrimitiveContainer();
            CopyField(from, to, FIELD_NAME);

            to.Field.ShouldBe(value);
        }

        [Test]
        public void CopyToObjectField()
        {
            var value = new object();

            var from = new TypeHelper.ObjectContainer();
            from.Field = value;

            var to = new TypeHelper.ObjectContainer();
            CopyField(from, to, FIELD_NAME);

            to.Field.ShouldBeSameAs(value);
        }

        [Test]
        public void ChangeField()
        {
            var reference = new TypeHelper.AllTypesContainer();
            var changed = new TypeHelper.AllTypesContainer();
            reference.ShouldBe(changed);

            foreach (var field  in typeof(TypeHelper.AllTypesContainer).GetFields(TypeHelper.DefaultBindingFlags)) {
                new FieldAccessor(changed, field).ChangeField(_prefabValues);
                TestFrameworkBridge.AssertFalse("On field: " + field.Name, reference.Equals(changed));

                new FieldAccessor(reference, field).ChangeField(_prefabValues);
                TestFrameworkBridge.AssertTrue("On field: " + field.Name, reference.Equals(changed));
            }
        }

        [Test]
        public void ChangeFieldOnPrimitiveStaticFinalIsNoOp()
        {
            var foo = new TypeHelper.StaticFinalContainer();
            ChangeField(foo, "CONST");
            TypeHelper.StaticFinalContainer.CONST.ShouldBe(42);
        }

        [Test]
        public void ChangeFieldStaticFinal()
        {
            var foo = new TypeHelper.StaticFinalContainer();
            var original = TypeHelper.StaticFinalContainer.OBJECT;
            ChangeField(foo, "OBJECT");
            TypeHelper.StaticFinalContainer.OBJECT.ShouldBeSameAs(original);
        }

        [Test]
        public void ChangeAbstractField()
        {
            var foo = new TypeHelper.AbstractClassContainer();
            ChangeField(foo, FIELD_NAME);
            foo.Field.ShouldNotBe(null);
        }

        [Test]
        public void ChangeInterfaceField()
        {
            var foo = new TypeHelper.InterfaceContainer();
            ChangeField(foo, FIELD_NAME);
            foo.Field.ShouldNotBe(null);
        }

        [Test]
        public void ChangeArrayField()
        {
            var reference = new TypeHelper.AllArrayTypesContainer();
            var changed = new TypeHelper.AllArrayTypesContainer();
            TestFrameworkBridge.AssertTrue("Before", reference.Equals(changed));

            foreach (var field in typeof(TypeHelper.AllArrayTypesContainer).GetFields(TypeHelper.DefaultBindingFlags)) {
                new FieldAccessor(changed, field).ChangeField(_prefabValues);
                TestFrameworkBridge.AssertFalse("On Field: " + field.Name, reference.Equals(changed));
                new FieldAccessor(reference, field).ChangeField(_prefabValues);
                TestFrameworkBridge.AssertTrue("On Field: " + field.Name, reference.Equals(changed));
            }
        }

        [Test]
        public void ChangeAbstractArrayField()
        {
            var foo = new TypeHelper.AbstractAndInterfaceArrayContainer();
            ChangeField(foo, "AbstractClasses");
            foo.AbstractClasses[0].ShouldNotBe(null);
        }

        [Test]
        public void ChangeInterfaceArrayField()
        {
            var foo = new TypeHelper.AbstractAndInterfaceArrayContainer();
            ChangeField(foo, "Interfaces");
            foo.Interfaces[0].ShouldNotBe(null);
        }

        [Test]
        public void AddPrefabValues()
        {
            var foo = new PointContainer(new Point(1, 2));
            _prefabValues.Put(typeof(Point), RED_NEW_POINT, BLACK_NEW_POINT);

            ChangeField(foo, "point");
            foo.Point.ShouldBe(RED_NEW_POINT);

            ChangeField(foo, "point");
            foo.Point.ShouldBe(BLACK_NEW_POINT);

            ChangeField(foo, "point");
            foo.Point.ShouldBe(RED_NEW_POINT);
        }

        [Test]
        public void AddPrefabArrayValues()
        {
            var foo = new TypeHelper.PointArrayContainer();
            _prefabValues.Put(typeof(Point), RED_NEW_POINT, BLACK_NEW_POINT);

            ChangeField(foo, "Points");
            foo.Points[0].ShouldBe(RED_NEW_POINT);

            ChangeField(foo, "Points");
            foo.Points[0].ShouldBe(BLACK_NEW_POINT);

            ChangeField(foo, "Points");
            foo.Points[0].ShouldBe(RED_NEW_POINT);
        }

        static object GetValue(object obj, string fieldName)
        {
            return GetAccessorFor(obj, fieldName).Get();
        }

        static void SetField(object obj, string fieldName, object value)
        {
            GetAccessorFor(obj, fieldName).Set(value);
        }

        static void DefaultField(object obj, string fieldName)
        {
            GetAccessorFor(obj, fieldName).DefaultField();
        }

        static void CopyField(object from, object to, string fieldName)
        {
            GetAccessorFor(from, fieldName).CopyTo(to);
        }

        void ChangeField(object obj, string fieldName)
        {
            GetAccessorFor(obj, fieldName).ChangeField(_prefabValues);
        }

        static FieldAccessor GetAccessorFor(object obj, string fieldName)
        {
            try {
                var field = obj.GetType().GetField(fieldName, FieldHelper.DeclaredOnly);
                if (field == null)
                    throw new ArgumentException("fieldName: " + fieldName);

                return new FieldAccessor(obj, field);
            }
            catch {
                throw new ArgumentException("fieldName: " + fieldName);
            }
        }

    }
}

