namespace EqualsVerifier.TestHelpers.Types
{
    public class Multiple
    {
        readonly int a;
        readonly int b;

        public Multiple(int a, int b)
        {
            this.a = a;
            this.b = b;
        }

        public sealed override bool Equals(object obj)
        {
            var other = obj as Multiple;
            if (other == null)
                return false;

            return a * b == other.a * other.b;
        }

        public sealed override int GetHashCode()
        {
            return a * b;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}*{2}={3}", GetType().Name, a, b, (a * b));
        }
    }
}

