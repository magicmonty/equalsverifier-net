using System;
using System.Collections;
using System.Linq;

namespace EqualsVerifier.Util
{
    public static class ArrayExtensions
    {
        const int Prime = 31;

        public static int GetShallowHashCode(this object array)
        {
            unchecked
            {
                if (ReferenceEquals(null, array))
                    return 0;
                
                if (!array.GetType().IsArray)
                    return array.GetHashCode();
                
                var result = 0;
                foreach (var element in ToArray(array))
                {
                    if (element == null)
                        continue;
                    
                    result += element.GetHashCode() * Prime;
                }
                
                return result;
            }
        }

        public static int GetDeepHashCode(this object array)
        {
            unchecked
            {
                if (ReferenceEquals(null, array))
                    return 0;
                
                if (!array.GetType().IsArray)
                    return array.GetHashCode();
                
                var result = 0;
                foreach (var element in ToArray(array))
                {
                    if (element == null)
                        continue;
                    
                    result += element.GetDeepHashCode() * Prime;
                }
                
                return result;
            }
        }

        public static bool ArrayShallowEquals(this object x, object y)
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
            foreach (var ex in ax)
            {
                var ey = ay[i++];
                if (ex == null && ey == null)
                    continue;

                if (ex != null && ex == null || ex == null && ey != null)
                    return false;

                result &= Object.Equals(ex, ey);

                if (!result)
                    break;
            }

            return result;
        }

        public static bool ArrayDeeplyEquals(this object x, object y)
        {
            if (ReferenceEquals(x, null) && ReferenceEquals(y, null))
                return true;

            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

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
            foreach (var ex in ax)
            {
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

        public static object[] ToArray(object obj)
        {
            return (obj as IEnumerable).Cast<object>().ToArray();
        }
    }
}

