using System.Reflection;

namespace EqualsVerifier.Util
{
    public static class FieldHelper
    {
        public static readonly BindingFlags AllFields = 
            BindingFlags.Static
            | BindingFlags.Instance
            | BindingFlags.NonPublic
            | BindingFlags.Public;

        public static readonly BindingFlags DeclaredOnly = AllFields | BindingFlags.DeclaredOnly;
    }
}

