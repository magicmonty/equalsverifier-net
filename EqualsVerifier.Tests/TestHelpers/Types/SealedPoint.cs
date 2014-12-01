namespace EqualsVerifier.Tests.TestHelpers.Types
{
    public sealed class SealedPoint
    {
        readonly int _x;
        readonly int _y;

        public SealedPoint(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public override bool Equals(object obj)
        {
            var other = obj as SealedPoint;
            if (other == null)
                return false;

            return other._x == _x
            && other._y == _y;
        }

        public override int GetHashCode()
        {
            return _x + (31 * _y);
        }

        public override string ToString()
        {
            return string.Format("{0}:{1},{2}", GetType().Name, _x, _y);
        }
    }
}

