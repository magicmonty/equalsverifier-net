using System;
using System.Reflection;
using System.Linq;

namespace EqualsVerifier.Util
{
    public class ObjectAccessor
    {
        readonly object _object;
        readonly Type _type;

        public static ObjectAccessor Of(object obj)
        {
            return new ObjectAccessor(obj, obj.GetType());
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

        public object CopyIntoAnonymousSubclass()
        {
            var copy = Instantiator.InstantiateAnonymousSubclass(_type);
            return CopyInto(copy);
        }

        object CopyInto(object copy)
        {
            FieldEnumerable
                .Of(_type)
                .Select(f => new FieldAccessor(_object, f))
                .ToList()
                .ForEach(accessor => accessor.CopyTo(copy));

            return copy;
        }

        public void Scramble(PrefabValues prefabValues)
        {
            var fields = FieldEnumerable
                .Of(_type)
                .Select(f => new FieldAccessor(_object, f))
                .ToList();

            fields.ForEach(accessor => accessor.ChangeField(prefabValues));
        }

        public void ShallowScramble(PrefabValues prefabValues)
        {
            var fields = FieldEnumerable
                .OfIgnoringSuper(_type)
                .Select(f => new FieldAccessor(_object, f))
                .ToList();

            fields.ForEach(accessor => accessor.ChangeField(prefabValues));
        }
    }
}

