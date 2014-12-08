using System.Reflection;
using System;
using EqualsVerifier.Util.Exceptions;

namespace EqualsVerifier.Util
{
    public class FieldAccessor
    {
        readonly object _object;
        readonly FieldInfo _field;

        public FieldAccessor(object obj, FieldInfo field)
        {
            _object = obj;
            _field = field;
        }

        public object Object { get { return _object; } }

        public FieldInfo Field { get { return _field; } }

        public Type FieldType { get { return _field.FieldType; } }

        public string FieldName { get { return _field.Name; } }

        public bool IsPrimitive { get { return FieldType.IsPrimitive; } }

        public bool IsReadonly { get { return _field.IsInitOnly; } }

        public bool IsStatic { get { return _field.IsStatic; } }

        public object Get()
        {
            try
            {
                return _field.GetValue(_object);
            }
            catch (Exception e)
            {
                throw new ReflectionException(e);
            }
        }

        public void Set(object value)
        {
            Modify(new FieldSetter(_field, _object, value));
        }

        public void DefaultField()
        {
            Modify(new FieldDefaulter(_field, _object));
        }

        public void CopyTo(object to)
        {
            Modify(new FieldCopier(_field, _object, to));
        }

        public void ChangeField(PrefabValues prefabValues)
        {
            Modify(new FieldChanger(_field, _object, prefabValues));
        }

        void Modify(IFieldModifier modifier)
        {
            if (!CanBeModifiedReflectively())
                return;

            try
            {
                modifier.Modify();
            }
            catch (FieldAccessException e)
            {
                throw new ReflectionException(e);
            }
            catch (MethodAccessException e)
            {
                throw new ReflectionException(e);
            }
        }

        public bool CanBeModifiedReflectively()
        {
            if (_field.IsLiteral || (IsReadonly && IsStatic))
                return false;
            

            // Castle.DynamicProxy, which is used by this class, adds several fields to classes
            // that it creates. If they are changed using reflection, exceptions
            // are thrown.
            return !_field.FieldType.FullName.StartsWith("Castle", StringComparison.InvariantCulture);
        }

        static void CreatePrefabValues(PrefabValues prefabValues, Type type)
        {
            prefabValues.PutFor(type);
        }

        internal interface IFieldModifier
        {
            void Modify();
        }

        class FieldSetter : IFieldModifier
        {
            readonly object _newValue;
            readonly object _object;
            readonly FieldInfo _field;

            public FieldSetter(FieldInfo field, object obj, object newValue)
            {
                _newValue = newValue;
                _object = obj;
                _field = field;
            }

            public void Modify()
            {
                this._field.SetValue(this._object, _newValue);
            }
        }

        class FieldDefaulter : IFieldModifier
        {
            readonly object _object;
            readonly FieldInfo _field;

            public FieldDefaulter(FieldInfo field, object obj)
            {
                _object = obj;
                _field = field;
            }

            public void Modify()
            {
                var type = this._field.FieldType;
                if (type.IsValueType)
                    this._field.SetValue(this._object, Activator.CreateInstance(type));
                else
                    this._field.SetValue(this._object, null);
            }
        }

        class FieldCopier: IFieldModifier
        {
            readonly object _to;
            readonly object _object;
            readonly FieldInfo _field;

            public FieldCopier(FieldInfo field, object obj, object to)
            {
                _to = to;
                _field = field;
                _object = obj;
            }

            public void Modify()
            {
                this._field.SetValue(_to, this._field.GetValue(this._object));
            }
        }

        class FieldChanger: IFieldModifier
        {
            readonly PrefabValues _prefabValues;
            readonly object _object;
            readonly FieldInfo _field;

            public FieldChanger(FieldInfo field, object obj, PrefabValues prefabValues)
            {
                _prefabValues = prefabValues;
                _field = field;
                _object = obj;
            }

            public void Modify()
            {
                var type = _field.FieldType;

                if (_prefabValues.Contains(type))
                {
                    var newValue = _prefabValues.GetOther(type, _field.GetValue(_object));
                    _field.SetValue(_object, newValue);
                }
                else
                {
                    CreatePrefabValues(_prefabValues, type);
                    if (!_prefabValues.Contains(type))
                        return;
                    var newValue = _prefabValues.GetOther(type, _field.GetValue(_object));
                    _field.SetValue(_object, newValue);
                }
            }
        }
    }
}

