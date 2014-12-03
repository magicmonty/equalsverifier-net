using EqualsVerifier.Util;
using System;

namespace EqualsVerifier
{
    public class NetApiPrefabValues
    {
        readonly PrefabValues _prefabValues;

        public static void AddTo(PrefabValues prefabValues)
        {
            new NetApiPrefabValues(prefabValues).AddFrameworkClasses();
        }

        NetApiPrefabValues(PrefabValues prefabValues)
        {
            _prefabValues = prefabValues;
        }

        void AddFrameworkClasses()
        {
            AddPrimitiveClasses();
        }

        void AddPrimitiveClasses()
        {
            _prefabValues.Put(typeof(bool), true, false);
            _prefabValues.Put(typeof(byte), (byte)1, (byte)2);
            _prefabValues.Put(typeof(char), 'a', 'b');
            _prefabValues.Put(typeof(double), 0.5, 1.0);
            _prefabValues.Put(typeof(float), 0.5f, 1.0f);
            _prefabValues.Put(typeof(int), 1, 2);
            _prefabValues.Put(typeof(long), 1L, 2L);
            _prefabValues.Put(typeof(short), (short)1, (short)2);
            _prefabValues.Put(typeof(string), "one", "two");

            // _prefabValues.Put(typeof(Boolean), true, false);
            // _prefabValues.Put(typeof(Byte), (byte)1, (byte)2);
            // _prefabValues.Put(typeof(Char), 'a', 'b');
            // _prefabValues.Put(typeof(Double), 0.5, 1.0);
            // _prefabValues.Put(typeof(Int32), 1, 2);
            // _prefabValues.Put(typeof(Int64), 1L, 2L);
            // _prefabValues.Put(typeof(Int16), (short)1, (short)2);
            // _prefabValues.Put(typeof(String), "one", "two");

            _prefabValues.Put(typeof(object), new object(), new object());
            _prefabValues.Put(typeof(Type), typeof(object), typeof(string));
        }
    }
}

