using System;
using System.Linq;
using System.Collections.Generic;
using EqualsVerifier.Util.Exceptions;
using System.Reflection;
using System.Collections;

namespace EqualsVerifier.Util
{
    public class PrefabValues
    {
        readonly StaticFieldValueStash _stash;
        readonly Dictionary<Type, RedBlackTuple> _values = new Dictionary<Type, RedBlackTuple>();

        public PrefabValues(StaticFieldValueStash stash)
        {
            _stash = stash;
        }

        public void Put<T>(Type type, T red, T black)
        {
            if (object.Equals(red, black))
                throw new ArgumentException("red equals black");

            _values.Add(type, new RedBlackTuple(red, black));
        }

        public void PutAll(PrefabValues from)
        {
            foreach (var kvp in from._values)
                _values.Add(kvp.Key, kvp.Value);
        }

        public bool Contains(Type type)
        {
            return _values.ContainsKey(type);
        }

        public object GetRed(Type type)
        {
            return GetTuple(type).Red;
        }

        public object GetBlack(Type type)
        {
            return GetTuple(type).Black;
        }

        RedBlackTuple GetTuple(Type type)
        {
            return _values[type];
        }

        public object GetOther(Type type, object value)
        {
            if (type == null)
            {
                throw new ReflectionException("Type is null.");
            }

            if (value != null && !type.IsAssignableFrom(value.GetType()) && !Wraps(type, value.GetType()))
            {
                throw new ReflectionException("Type does not match value.");
            }

            if (!_values.ContainsKey(type))
                throw new ReflectionException("No prefab values for " + type + " exist.");

            var tuple = _values[type];

            if (type.IsArray && tuple.Red.ArrayDeeplyEquals(value))
            {
                return tuple.Black;
            }

            if (!type.IsArray && tuple.Red.Equals(value))
            {
                return tuple.Black;
            }

            return tuple.Red;
        }

        static bool Wraps(Type expectedClass, Type actualClass)
        {
            return
                (expectedClass.Equals(typeof(bool)) && actualClass.Equals(typeof(Boolean))) ||
            (expectedClass.Equals(typeof(byte)) && actualClass.Equals(typeof(Byte))) ||
            (expectedClass.Equals(typeof(char)) && actualClass.Equals(typeof(Char))) ||
            (expectedClass.Equals(typeof(double)) && actualClass.Equals(typeof(Double))) ||
            (expectedClass.Equals(typeof(float)) && actualClass.Equals(typeof(float))) ||
            (expectedClass.Equals(typeof(int)) && actualClass.Equals(typeof(Int32))) ||
            (expectedClass.Equals(typeof(long)) && actualClass.Equals(typeof(Int64))) ||
            (expectedClass.Equals(typeof(short)) && actualClass.Equals(typeof(Int16)));
        }

        static object[] ToArray(object obj)
        {
            return (obj as IEnumerable).Cast<object>().ToArray();
        }

        public void PutFor(Type type)
        {
            PutFor(type, new Stack<Type>());
        }

        void PutFor(Type type, Stack<Type> typeStack)
        {
            if (NoNeedToCreatePrefabValues(type))
                return;

            if (typeStack.Contains(type))
            {
                throw new RecursionException(typeStack);
            }

            _stash.Backup(type);
            var clone = new Stack<Type>(typeStack);
            clone.Push(type);

            if (type.IsEnum)
                PutEnumInstances(type);
            else if (type.IsArray)
                PutArrayInstances(type, clone);
            else
            {
                TraverseFields(type, clone);
                CreateAndPutInstances(type);
            }
        }

        bool NoNeedToCreatePrefabValues(Type type)
        {
            return Contains(type) || type.IsPrimitive;
        }

        void PutEnumInstances(Type type)
        {

            var enumConstants = type.GetEnumValues();

            switch (enumConstants.Length)
            {
                case 0:
                    throw new ReflectionException("Enum " + type.Name + " has no elements");
                case 1:
                    Put(type, enumConstants.GetValue(0), null);
                    break;
                default:
                    Put(type, enumConstants.GetValue(0), enumConstants.GetValue(1));
                    break;
            }
        }

        void PutArrayInstances(Type type, Stack<Type> typeStack)
        {
            var componentType = type.GetElementType();
            PutFor(componentType, typeStack);

            var red = Array.CreateInstance(componentType, 1);
            red.SetValue(GetRed(componentType), 0);

            var black = Array.CreateInstance(componentType, 1);
            black.SetValue(GetBlack(componentType), 0);

            Put(type, red, black);
        }

        void TraverseFields(Type type, Stack<Type> typeStack)
        {
            FieldEnumerable
                .Of(type)
                .Where(f => !f.IsInitOnly)
                .Select(f => f.FieldType)
                .ToList()
                .ForEach(t => PutFor(t, typeStack));
        }

        void CreateAndPutInstances(Type type)
        {
            try
            {
                var accessor = ClassAccessor.Of(type, this, false);
                var red = accessor.GetRedObject();
                var black = accessor.GetBlackObject();
                
                Put(type, red, black);
            }
            catch (TargetInvocationException ex)
            {
                if (!(ex.InnerException is NullReferenceException))
                    throw ex;

                throw new ReflectionException(
                    ObjectFormatter.Of(
                        "Could not reliable intantiate examples for type '%%'! Please provide examples by yourself!",
                        type)
                    .Format());
            }
        }

        class RedBlackTuple : Tuple<object, object>
        {
            public RedBlackTuple(object red, object black) : base(red, black)
            {
            }

            public object Red { get { return Item1; } }

            public object Black { get { return Item2; } }
        }
    }
}

