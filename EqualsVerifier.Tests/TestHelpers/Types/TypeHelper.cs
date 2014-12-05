using System;
using System.Linq;
using EqualsVerifier.Util;
using System.Reflection;
using Mono.Math;
using System.Globalization;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Collections;

namespace EqualsVerifier.TestHelpers.Types
{
    public static class TypeHelper
    {
        public interface IInterface
        {

        }

        static readonly object OBJECT = new object();

        public static readonly BindingFlags DefaultBindingFlags = 
            BindingFlags.Static
            | BindingFlags.Instance
            | BindingFlags.DeclaredOnly
            | BindingFlags.NonPublic
            | BindingFlags.Public;

        public enum Enum
        {
            FIRST,
            SECOND

        }

        #pragma warning disable 659
        public class AllTypesContainer
        {
            public bool _boolean = false;
            public byte _byte = 0;
            public char _char = '\u0000';
            public double _double = 0.0D;
            public float _float = 0.0F;
            public int _int = 0;
            public long _long = 0L;
            public short _short = 0;

            public Enum _enum = Enum.FIRST;
            public int[] _array = { 1, 2, 3 };
            public Object _object = OBJECT;
            public Type _type = typeof(object);
            public String _string = string.Empty;

            public override bool Equals(object obj)
            {
                var other = obj as AllTypesContainer;
                if (other == null)
                    return false;

                var result = true;

                result &= _boolean == other._boolean;
                result &= _byte == other._byte;
                result &= _char == other._char;
                result &= Math.Abs(_double - other._double) < 0.0000001;
                result &= Math.Abs(_float - other._float) < 0.0000001;
                result &= _int == other._int;
                result &= _long == other._long;
                result &= _short == other._short;
                result &= _enum == other._enum;
                result &= _array == null && other._array == null
                || (_array != null
                && other._array != null
                && _array.Length == other._array.Length
                && _array.SequenceEqual(other._array));
                result &= _object.NullSafeEquals(other._object);
                result &= _type == other._type;
                result &= String.Equals(_string, other._string);

                return result;
            }
        }

        public class AllArrayTypesContainer
        {
            public bool[] Booleans = { true };
            public byte[] Bytes = { (byte)1 };
            public char[] Chars = { 'a' };
            public double[] Doubles = { 1.0 };
            public float[] Floats = { 1.0f };
            public int[] Ints = { 1 };
            public long[] Longs = { 1L };
            public short[] Shorts = { (short)1 };

            public Enum[] Enums = { Enum.FIRST };
            public int[][] Arrays = { new int[] { 1 } };
            public object[] Objects = { OBJECT };
            public Type[] Types = { typeof(object) };
            public string[] Strings = { "1" };

            public override bool Equals(object obj)
            {
                var other = obj as AllArrayTypesContainer;
                if (other == null)
                    return false;

                var result = true;
                result &= Booleans.ArrayDeeplyEquals(other.Booleans);
                result &= Bytes.ArrayDeeplyEquals(other.Bytes);
                result &= Chars.ArrayDeeplyEquals(other.Chars);
                result &= Doubles.ArrayDeeplyEquals(other.Doubles);
                result &= Floats.ArrayDeeplyEquals(other.Floats);
                result &= Ints.ArrayDeeplyEquals(other.Ints);
                result &= Longs.ArrayDeeplyEquals(other.Longs);
                result &= Shorts.ArrayDeeplyEquals(other.Shorts);
                result &= Enums.ArrayDeeplyEquals(other.Enums);
                result &= Arrays.ArrayDeeplyEquals(other.Arrays);
                result &= Objects.ArrayDeeplyEquals(other.Objects);
                result &= Types.ArrayDeeplyEquals(other.Types);
                result &= Strings.ArrayDeeplyEquals(other.Strings);

                return result;
            }
        }
        #pragma warning restore 659

        public class Empty
        {

        }

        public abstract class AbstractClass
        {
            public int Field;
        }

        public class AbstractClassContainer
        {
            public AbstractClass Field;
        }

        public class InterfaceContainer
        {
            public IInterface Field;
        }

        public class NoFields
        {

        }

        public class NoFieldsSubWithFields : NoFields
        {
            public object Field;
        }

        public class ArrayContainer
        {
            public int[] Field;
        }

        public class ObjectContainer
        {
            public object Field = new object();
        }

        public class PrimitiveContainer
        {
            public int Field;
        }

        #pragma warning disable 414
        public class ReadonlyContainer
        {
            readonly object Field = new object();
        }

        public class StaticContainer
        {
            static object Field = new object();
        }
        #pragma warning restore 414

        public class PrivateObjectContainer
        {
            object Field = new object();

            public object Get()
            {
                return Field;
            }
        }

        public class StaticFinalContainer
        {
            public static readonly int CONST = 42;
            public static readonly object OBJECT = new object();
        }

        public class Outer
        {
            public class Inner
            {
                readonly Outer _outer;

                public Inner(Outer outer)
                {
                    _outer = outer;
                }

                public Outer GetOuter()
                {
                    return _outer;
                }
            }
        }

        public class AbstractAndInterfaceArrayContainer
        {
            public AbstractClass[] AbstractClasses = { null };
            public IInterface[] Interfaces = { null };
        }

        public class PointArrayContainer
        {
            public Point[] Points = { new Point(1, 2) };
        }

        public abstract class AbstractEqualsAndHashCode
        {
            public override abstract bool Equals(object obj);

            public override abstract int GetHashCode();
        }

        public class RecursiveApiClassesContainer
        {
            public BigInteger bigInteger;
            public DateTime date;
        }

        #pragma warning disable 169
        public class AllRecursiveCollectionImplementationsContainer
        {
            LinkedList<object> linkedList;
            // Dictionary<object, object> dictionary;
            // HashSet<object> hashSet;
            List<object> list;
            Queue<object> queue;
            // Stack<object> stack;
            // SortedList sortedList;
            // SortedSet<object> sortedSet;
        }
        #pragma warning restore 169

        public class DifferentAccessModifiersFieldContainer
        {
            private readonly int i = 0;
            internal readonly int j = 0;
            protected readonly int k = 0;
            public readonly int l = 0;

            private static readonly int I = 0;
            internal static int J = 0;
            protected static readonly int K = 0;
            public static readonly int L = 0;
        }

        public class DifferentAccessModifiersSubFieldContainer : DifferentAccessModifiersFieldContainer
        {
            private readonly string a = "";
            internal readonly string b = "";
            protected readonly string c = "";
            public readonly string d = "";
        }

        public class EmptySubFieldContainer : DifferentAccessModifiersFieldContainer
        {

        }

        public class SubEmptySubFieldContainer : EmptySubFieldContainer
        {
            public long field = 0;
        }
    }
}

