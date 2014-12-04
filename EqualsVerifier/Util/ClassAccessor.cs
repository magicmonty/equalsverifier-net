using System;
using System.Reflection;
using EqualsVerifier.Util.Exceptions;

namespace EqualsVerifier.Util
{
    public class ClassAccessor
    {
        readonly Type _type;
        readonly PrefabValues _prefabValues;

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
                return _type.GetField(field.Name, FieldHelper.DeclaredOnly) != null;
            }
            catch {
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
                return _type.GetMethod(
                    name, 
                    FieldHelper.DeclaredOnly,
                    null,
                    parameterTypes,
                    null) != null;
            }
            catch {
                return false;
            }
        }

        public bool IsEqualsAbstract { get { return IsMethodAbstract("Equals", typeof(object)); } }

        public bool IsGetHashCodeAbstract { get { return IsMethodAbstract("GetHashCode"); } }

        bool IsMethodAbstract(string name, params Type[] parameterTypes)
        {
            if (!DeclaresMethod(name, parameterTypes))
                return false;

            try {
                return _type.GetMethod(name, parameterTypes).IsAbstract;
            }
            catch {
                throw new ReflectionException("Should never occur (famous last words)");
            }
        }

        public bool IsEqualsInheritedFromObject {
            get
            {
                var currentAccessor = this;
                while (currentAccessor.Type != typeof(object)) {
                    if (currentAccessor.DeclaresEquals() && !currentAccessor.IsEqualsAbstract)
                        return false;

                    currentAccessor = currentAccessor.GetSuperAccessor();
                }

                return true;
            }
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

