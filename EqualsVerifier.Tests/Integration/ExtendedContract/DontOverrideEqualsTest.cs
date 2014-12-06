using NUnit.Framework;

namespace EqualsVerifier.Integration.ExtendedContract
{
    [TestFixture]
    public class DontOverrideEqualsTest
    {

        [Test]
        public void WhenClassDoesntOverrideEqualsOrHashCode_ThenSucceed()
        {
            EqualsVerifier.ForType<Poco>()
                .Suppress(Warning.NONFINAL_FIELDS)
                .Verify();
        }

        public sealed class Poco
        {
            string _value;

            public string Value
            {
                get { return _value; }
                set { _value = value; }
            }

            public override string ToString()
            {
                return GetType().Name + " " + (_value ?? string.Empty);
            }
        }
    }
}

