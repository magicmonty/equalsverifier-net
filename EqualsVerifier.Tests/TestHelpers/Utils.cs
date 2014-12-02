using System;

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
    }
}

