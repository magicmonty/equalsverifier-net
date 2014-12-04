using EqualsVerifier.Util;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;


namespace EqualsVerifier.Checker
{
    public class FieldsChecker<T> : AbstractChecker
    {
        readonly ClassAccessor _classAccessor;
        readonly ISet<Warning> _warningsToSuppress;
        readonly bool _allFieldsShouldBeUsed;
        readonly ISet<string> _allFieldsShouldBeUsedExceptions;
        readonly PrefabValues _prefabValues;

        public FieldsChecker(
            ClassAccessor classAccessor, 
            ISet<Warning> warningsToSuppress, 
            bool allFieldsShouldBeUsed, 
            ISet<string> allFieldsShouldBeUsedExceptions)
        {
            _classAccessor = classAccessor;
            _prefabValues = classAccessor.PrefabValues;
            _warningsToSuppress = new HashSet<Warning>(warningsToSuppress);
            _allFieldsShouldBeUsed = allFieldsShouldBeUsed;
            _allFieldsShouldBeUsedExceptions = allFieldsShouldBeUsedExceptions;
        }

        public override void Check()
        {
            var inspector = new FieldInspector<T>(_classAccessor);

            if (_classAccessor.DeclaresEquals) {
                inspector.Check(new ArrayFieldCheck());
                inspector.Check(new FloatAndDoubleFieldCheck());
                inspector.Check(new ReflexivityFieldCheck(_classAccessor, _prefabValues, _warningsToSuppress));
            }

            if (!IgnoreMutability()) {
                inspector.Check(new MutableStateFieldCheck(_prefabValues));
            }

            inspector.Check(new SignificantFieldCheck(_classAccessor, _prefabValues, _allFieldsShouldBeUsed, _allFieldsShouldBeUsedExceptions));
            inspector.Check(new SymmetryFieldCheck(_prefabValues));
            inspector.Check(new TransitivityFieldCheck(_prefabValues));
        }

        bool IgnoreMutability()
        {
            return _warningsToSuppress.Contains(Warning.NONFINAL_FIELDS) ||
            _classAccessor.HasAttribute(SupportedAttributes.IMMUTABLE);
        }

        class SymmetryFieldCheck : IFieldCheck
        {
            readonly PrefabValues _prefabValues;

            public SymmetryFieldCheck(PrefabValues prefabValues)
            {
                _prefabValues = prefabValues;
            }

            public void Execute(FieldAccessor referenceAccessor, FieldAccessor changedAccessor)
            {
                CheckSymmetry(referenceAccessor, changedAccessor);

                changedAccessor.ChangeField(_prefabValues);
                CheckSymmetry(referenceAccessor, changedAccessor);

                referenceAccessor.ChangeField(_prefabValues);
                CheckSymmetry(referenceAccessor, changedAccessor);
            }

            static void CheckSymmetry(FieldAccessor referenceAccessor, FieldAccessor changedAccessor)
            {
                var left = referenceAccessor.Object;
                var right = changedAccessor.Object;
                AssertTrue(
                    ObjectFormatter.Of("Symmetry: objects are not symmetric:\n  %%\nand\n  %%", left, right),
                    left.Equals(right) == right.Equals(left));
            }
        }

        class TransitivityFieldCheck: IFieldCheck
        {
            readonly PrefabValues _prefabValues;

            public TransitivityFieldCheck(PrefabValues prefabValues)
            {
                _prefabValues = prefabValues;
            }

            public void Execute(FieldAccessor referenceAccessor, FieldAccessor changedAccessor)
            {
                var a1 = referenceAccessor.Object;
                var b1 = BuildB1(changedAccessor);
                var b2 = BuildB2(a1, referenceAccessor.Field);

                var x = a1.Equals(b1);
                var y = b1.Equals(b2);
                var z = a1.Equals(b2);

                if (CountFalses(x, y, z) == 1) {
                    TestFrameworkBridge.Fail(ObjectFormatter.Of("Transitivity: two of these three instances are equal to each other, so the third one should be, too:\n-  %%\n-  %%\n-  %%", a1, b1, b2));
                }
            }

            object BuildB1(FieldAccessor accessor)
            {
                accessor.ChangeField(_prefabValues);
                return accessor.Object;
            }

            object BuildB2(object a1, FieldInfo referenceField)
            {
                var result = ObjectAccessor.Of(a1).Copy();
                var objectAccessor = ObjectAccessor.Of(result);
                objectAccessor.FieldAccessorFor(referenceField).ChangeField(_prefabValues);

                foreach (var field in result.GetType().GetFields(FieldHelper.AllFields)) {
                    if (!field.Equals(referenceField)) {
                        objectAccessor.FieldAccessorFor(field).ChangeField(_prefabValues);
                    }
                }

                return result;
            }

            static int CountFalses(params bool[] bools)
            {
                return bools.Count(b => !b);
            }
        }

        class SignificantFieldCheck : IFieldCheck
        {
            readonly PrefabValues _prefabValues;
            readonly bool _allFieldsShouldBeUsed;
            readonly ISet<string> _allFieldsShouldBeUsedExceptions;
            readonly ClassAccessor _classAccessor;

            public SignificantFieldCheck(ClassAccessor classAccessor, PrefabValues prefabValues, bool allFieldsShouldBeUsed, ISet<string> allFieldsShouldBeUsedExceptions)
            {
                _classAccessor = classAccessor;
                _prefabValues = prefabValues;
                _allFieldsShouldBeUsed = allFieldsShouldBeUsed;
                _allFieldsShouldBeUsedExceptions = allFieldsShouldBeUsedExceptions;
            }

            public void Execute(FieldAccessor referenceAccessor, FieldAccessor changedAccessor)
            {
                var reference = referenceAccessor.Object;
                var changed = changedAccessor.Object;
                var fieldName = referenceAccessor.FieldName;


                changedAccessor.ChangeField(_prefabValues);

                var equalsChanged = !reference.Equals(changed);
                var hashCodeChanged = reference.GetHashCode() != changed.GetHashCode();

                if (equalsChanged != hashCodeChanged) {
                    AssertFalse(
                        ObjectFormatter.Of("Significant fields: equals relies on %%, but hashCode does not.", fieldName),
                        equalsChanged);
                    AssertFalse(ObjectFormatter.Of("Significant fields: hashCode relies on %%, but equals does not.", fieldName),
                        hashCodeChanged);
                }

                if (_allFieldsShouldBeUsed && !referenceAccessor.IsStatic) {
                    var thisFieldShouldBeUsed = _allFieldsShouldBeUsed && !_allFieldsShouldBeUsedExceptions.Contains(fieldName);
                    AssertTrue(
                        ObjectFormatter.Of("Significant fields: equals does not use %%.", fieldName),
                        !thisFieldShouldBeUsed || equalsChanged);

                    AssertTrue(ObjectFormatter.Of("Significant fields: equals should not use %%, but it does.", fieldName),
                        thisFieldShouldBeUsed || !equalsChanged);

                    if (_classAccessor.DeclaresField(referenceAccessor.Field)) {
                        AssertTrue(
                            ObjectFormatter.Of("Significant fields: all fields should be used, but %% has not defined an equals method.", _classAccessor.Type.Name),
                            _classAccessor.DeclaresEquals());
                    }
                }

                referenceAccessor.ChangeField(_prefabValues);
            }
        }

        class ArrayFieldCheck: IFieldCheck
        {
            public void Execute(FieldAccessor referenceAccessor, FieldAccessor changedAccessor)
            {
                var arrayType = referenceAccessor.FieldType;
                if (!arrayType.IsArray)
                    return;

                var fieldName = referenceAccessor.FieldName;
                var reference = referenceAccessor.Object;
                var changed = changedAccessor.Object;
                ReplaceInnermostArrayValue(changedAccessor);

                if (arrayType.GetElementType().IsArray) {
                    AssertDeep(fieldName, reference, changed);
                }
                else {
                    AssertArray(fieldName, reference, changed);
                }
            }

            void ReplaceInnermostArrayValue(FieldAccessor accessor)
            {
                var newArray = ArrayCopy(accessor.Get());
                accessor.Set(newArray);
            }

            object ArrayCopy(object arrayObject)
            {
                var array = arrayObject.ToArray();
                var componentType = arrayObject.GetType().GetElementType();
                var result = Array.CreateInstance(componentType, 1);
                if (componentType.IsArray) {
                    result.SetValue(ArrayCopy(array.GetValue(0)), 0);
                }
                else {
                    result.SetValue(array.GetValue(0), 0);
                }
                return result;
            }

            static void AssertDeep(string fieldName, object reference, object changed)
            {
                TestFrameworkBridge.AssertEquals(
                    ObjectFormatter.Of("Multidimensional array: ==, regular equals() or Arrays.equals() used instead of Arrays.deepEquals() for field %%.", fieldName),
                    reference, changed);

                TestFrameworkBridge.AssertEquals(
                    ObjectFormatter.Of("Multidimensional array: regular hashCode() or Arrays.hashCode() used instead of Arrays.deepHashCode() for field %%.", fieldName),
                    reference.GetHashCode(), changed.GetHashCode());
            }

            static void AssertArray(string fieldName, object reference, object changed)
            {
                TestFrameworkBridge.AssertEquals(
                    ObjectFormatter.Of("Array: == or regular equals() used instead of Arrays.equals() for field %%.", fieldName),
                    reference, changed);

                TestFrameworkBridge.AssertEquals(
                    ObjectFormatter.Of("Array: regular hashCode() used instead of Arrays.hashCode() for field %%.", fieldName),
                    reference.GetHashCode(), changed.GetHashCode());
            }
        }

        class FloatAndDoubleFieldCheck : IFieldCheck
        {
            public void Execute(FieldAccessor referenceAccessor, FieldAccessor changedAccessor)
            {
                var type = referenceAccessor.FieldType;

                if (IsFloat(type)) {
                    referenceAccessor.Set(float.NaN);
                    changedAccessor.Set(float.NaN);
                    TestFrameworkBridge.AssertEquals(
                        ObjectFormatter.Of("Float: equals doesn't use Float.compare for field %%.", referenceAccessor.FieldName),
                        referenceAccessor.Object, 
                        changedAccessor.Object);
                }

                if (IsDouble(type)) {
                    referenceAccessor.Set(double.NaN);
                    changedAccessor.Set(double.NaN);
                    TestFrameworkBridge.AssertEquals(
                        ObjectFormatter.Of("Double: equals doesn't use Double.compare for field %%.", referenceAccessor.FieldName),
                        referenceAccessor.Object, 
                        changedAccessor.Object);
                }
            }

            static bool IsFloat(Type type)
            {
                return type == typeof(float);
            }

            static bool IsDouble(Type type)
            {
                return type == typeof(double);
            }
        }

        class ReflexivityFieldCheck : IFieldCheck
        {
            readonly ClassAccessor _classAccessor;
            readonly ISet<Warning> _warningsToSuppress;
            readonly PrefabValues _prefabValues;

            public ReflexivityFieldCheck(ClassAccessor classAccessor, PrefabValues prefabValues, ISet<Warning> warningsToSuppress)
            {
                _classAccessor = classAccessor;
                _prefabValues = prefabValues;
                _warningsToSuppress = warningsToSuppress;
                
            }

            public void Execute(FieldAccessor referenceAccessor, FieldAccessor changedAccessor)
            {
                if (_warningsToSuppress.Contains(Warning.IDENTICAL_COPY_FOR_VERSIONED_ENTITY))
                    return;

                referenceAccessor.ChangeField(_prefabValues);
                changedAccessor.ChangeField(_prefabValues);
                CheckReflexivityFor(referenceAccessor, changedAccessor);

                var fieldIsPrimitive = referenceAccessor.IsPrimitive;
                var fieldIsNonNull = _classAccessor.FieldHasAttribute(referenceAccessor.Field, SupportedAttributes.NONNULL);
                var ignoreNull = fieldIsNonNull || _warningsToSuppress.Contains(Warning.NULL_FIELDS);
                if (fieldIsPrimitive || !ignoreNull) {
                    referenceAccessor.DefaultField();
                    changedAccessor.DefaultField();
                    CheckReflexivityFor(referenceAccessor, changedAccessor);
                }
            }

            void CheckReflexivityFor(FieldAccessor referenceAccessor, FieldAccessor changedAccessor)
            {
                var left = referenceAccessor.Object;
                var right = changedAccessor.Object;

                if (_warningsToSuppress.Contains(Warning.IDENTICAL_COPY)) {
                    TestFrameworkBridge.AssertFalse(
                        ObjectFormatter.Of("Unnecessary suppression: %%. Two identical copies are equal.", Warning.IDENTICAL_COPY.ToString()),
                        left.Equals(right));
                }
                else {
                    var f = ObjectFormatter.Of("Reflexivity: object does not equal an identical copy of itself:\n  %%" +
                            "\nIf this is intentional, consider suppressing Warning.%%", left, Warning.IDENTICAL_COPY.ToString());
                    TestFrameworkBridge.AssertEquals(f, left, right);
                }
            }
        }

        class MutableStateFieldCheck : IFieldCheck
        {
            readonly PrefabValues _prefabValues;

            public MutableStateFieldCheck(PrefabValues prefabValues)
            {
                _prefabValues = prefabValues;
            }

            public void Execute(FieldAccessor referenceAccessor, FieldAccessor changedAccessor)
            {
                var reference = referenceAccessor.Object;
                var changed = changedAccessor.Object;

                changedAccessor.ChangeField(_prefabValues);

                var equalsChanged = !reference.Equals(changed);

                if (equalsChanged && !referenceAccessor.IsReadonly) {
                    TestFrameworkBridge.Fail(
                        ObjectFormatter.Of("Mutability: equals depends on mutable field %%.", referenceAccessor.FieldName));
                }

                referenceAccessor.ChangeField(_prefabValues);
            }
        }
    }
}

