using System;
using System.Collections;
using System.Linq;

namespace EqualsVerifier.Util
{
    public static class ArrayExtensions
    {
        public static bool ArrayDeeplyEquals(this object x, object y)
        {
            var tx = x.GetType();
            var ty = y.GetType();

            if (!tx.IsArray || !ty.IsArray)
                return false;

            if (tx.GetElementType() != ty.GetElementType())
                return false;

            var ax = ToArray(x);
            var ay = ToArray(y);

            if (ax.Length != ay.Length)
                return false;
                
            if (ax.SequenceEqual(ay))
                return true;

            var result = true;
            var i = 0;
            var containsArrays = tx.GetElementType().IsArray;
            foreach (var ex in ax) {
                var ey = ay[i++];
                if (ex == null && ey == null)
                    continue;

                if (ex != null && ex == null || ex == null && ey != null)
                    return false;

                result &= containsArrays 
                    ? ex.ArrayDeeplyEquals(ey) 
                    : Object.Equals(ex, ey);

                if (!result)
                    break;
            }
                
            return result;
        }

        static object[] ToArray(object obj)
        {
            return (obj as IEnumerable).Cast<object>().ToArray();
        }
    }
}

