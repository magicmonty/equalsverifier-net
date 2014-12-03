using System;
using Castle.Components.DictionaryAdapter;
using System.Reflection;

namespace EqualsVerifier.Util
{
    public class ObjectAccessor
    {
        private readonly object _object;
        private readonly Type _type;

        public static ObjectAccessor Of(object obj)
        {
            var type = obj.GetType();
            return new ObjectAccessor(obj, type);
        }

        public static ObjectAccessor Of(object obj, Type type)
        {
            return new ObjectAccessor(obj, type);
        }

        ObjectAccessor(object obj, Type type)
        {
            _object = obj;
            _type = type;
        }

        public object Get()
        {
            return _object;
        }

        public FieldAccessor FieldAccessorFor(FieldInfo field)
        {
            return new FieldAccessor(_object, field);
        }

        public object Copy()
        {
            var copy = Instantiator.Instantiate(_type);

            return CopyInto(copy);
        }

        public object CopyIntoSubclass(Type subclass)
        {
            var copy = Instantiator.Instantiate(subclass);
            return CopyInto(copy);
        }

        public object copyIntoAnonymousSubclass()
        {
            var copy = Instantiator.InstantiateAnonymousSubclass(_type);
            return CopyInto(copy);
        }

        private object CopyInto(object copy)
        {
            foreach (var field in _type.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.NonPublic)) {
                var accessor = new FieldAccessor(_object, field);
                accessor.CopyTo(copy);
            }
            return copy;
        }

        public void Scramble(PrefabValues prefabValues)
        {
            foreach (var field in _type.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.NonPublic)) {
                var accessor = new FieldAccessor(_object, field);
                accessor.ChangeField(prefabValues);
            }
        }

        public void ShallowScramble(PrefabValues prefabValues)
        {
            foreach (var field in _type.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.NonPublic)) {
                var accessor = new FieldAccessor(_object, field);
                accessor.ChangeField(prefabValues);
            }
        }
    }
}

