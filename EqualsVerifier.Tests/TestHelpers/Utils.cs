using System;
using EqualsVerifier.Util;
using System.Reflection;

namespace EqualsVerifier.TestHelpers
{
    public static class Utils
    {
        public static int GetNullSafeHashCode(this Object x)
        {
            return ReferenceEquals(x, null) ? 0 : x.GetHashCode();
        }

        public static bool NullSafeEquals(this Object x, Object y)
        {
            return x == null ? y == null : x.Equals(y);
        }

        public static int GetDefaultHashCode(this object x)
        {
            int hash = 59;
            try
            {
                foreach (var f in x.GetType().GetFields(FieldHelper.DeclaredOnly))
                {
                    if (IsRelevant(x, f))
                    {
                        var val = f.GetValue(x);
                        hash += 59 * val.GetNullSafeHashCode();
                    }
                }
            }
            catch (FieldAccessException e)
            {
                TestFrameworkBridge.Fail(e.ToString());
            }

            return hash;
        }

        static bool IsRelevant(object x, FieldInfo f)
        {
            var acc = new FieldAccessor(x, f);
            return acc.CanBeModifiedReflectively() && !acc.IsStatic;
        }
    }
}

