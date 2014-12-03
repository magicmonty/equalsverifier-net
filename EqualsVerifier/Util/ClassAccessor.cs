using System;
using System.Reflection;
using EqualsVerifier.Util.Exceptions;

namespace EqualsVerifier.Util
{
    public class ClassAccessor
    {
        private readonly Type _type;
        private readonly PrefabValues _prefabValues;

        public static ClassAccessor Of(Type type, PrefabValues prefabValues)
        {
            return new ClassAccessor(type, prefabValues);
        }

        ClassAccessor(Type type, PrefabValues prefabValues)
        {
            _type = type;
            _prefabValues = prefabValues;
        }

        public Type Type { get { return _type; } }

        public PrefabValues PrefabValues { get { return _prefabValues; } }

        public bool DeclaresField(FieldInfo field)
        {
            try {
                return _type.GetField(field.Name) != null;
            }
            catch (Exception e) {
                return false;
            }
        }

        public bool DeclaresEquals()
        {
            return DeclaresMethod("Equals", typeof(object));
        }

        public bool DeclaresGetHashCode()
        {
            return DeclaresMethod("GetHashCode");
        }

        bool DeclaresMethod(string name, params Type[] parameterTypes)
        {
            try {
                return _type.GetMethod(name, parameterTypes) != null;
            }
            catch (Exception e) {
                return false;
            }
        }

        public bool IsEqualsAbstract()
        {
            return IsMethodAbstract("Equals", typeof(object));
        }

        public bool IsGetHashCodeAbstract()
        {
            return IsMethodAbstract("GetHashCode");
        }

        bool IsMethodAbstract(string name, params Type[] parameterTypes)
        {
            if (!DeclaresMethod(name, parameterTypes))
                return false;

            try {
                return _type.GetMethod(name, parameterTypes).IsAbstract;
            }
            catch (Exception e) {
                throw new ReflectionException("Should never occur (famous last words)");
            }
        }

        public bool IsEqualsInheritedFromObject()
        {
            var i = this;
            while (i.Type != typeof(object)) {
                if (i.DeclaresEquals() && !i.IsEqualsAbstract()) {
                    return false;
                }

                i = i.GetSuperAccessor();
            }

            return true;
        }

        public ClassAccessor GetSuperAccessor()
        {
            return ClassAccessor.Of(_type.BaseType, _prefabValues);
        }

        public object GetRedObject()
        {
            return GetRedAccessor().Get();
        }

        public ObjectAccessor GetRedAccessor()
        {
            var result = BuildObjectAccessor();
            result.Scramble(_prefabValues);

            return result;
        }

        public object GetBlackObject()
        {
            return GetBlackAccessor().Get();
        }

        public ObjectAccessor GetBlackAccessor()
        {
            var result = BuildObjectAccessor();

            result.Scramble(_prefabValues);
            result.Scramble(_prefabValues);

            return result;
        }

        public object GetDefaultValuesObject()
        {
            var result = Instantiator.Instantiate(_type);
            return result;
        }

        ObjectAccessor BuildObjectAccessor()
        {
            var obj = Instantiator.Instantiate(_type);

            return ObjectAccessor.Of(obj);
        }
    }
}

