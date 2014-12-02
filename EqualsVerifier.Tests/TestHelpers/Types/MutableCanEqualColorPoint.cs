namespace EqualsVerifier.TestHelpers.Types
{
    public sealed class MutableCanEqualColorPoint : ImmutableCanEqualPoint
    {
        Color _color;

        public MutableCanEqualColorPoint(int x, int y, Color color) : base(x, y)
        {
            _color = color;
        }

        public override bool CanEqual(object obj)
        {
            return obj is MutableCanEqualColorPoint;
        }

        public override bool Equals(object obj)
        {
            var other = obj as MutableCanEqualColorPoint;
            if (other == null)
                return false;

            return other.CanEqual(this)
            && base.Equals(other)
            && _color == other._color;
        }

        public override int GetHashCode()
        {
            return _color.GetNullSafeHashCode() + (31 * base.GetHashCode());
        }

        public override string ToString()
        {
            return string.Format("{0},{1}", base.ToString(), _color);
        }
    }
}

