namespace EqualsVerifier.TestHelpers.Types
{
    public sealed class CanEqualColorPoint: CanEqualPoint
    {
        readonly Color _color;

        public CanEqualColorPoint(int x, int y, Color color) : base(x, y)
        {
            _color = color;
        }


        public override bool CanEqual(object obj)
        {
            return obj is CanEqualColorPoint;
        }

        public override bool Equals(object obj)
        {
            var canEqualColorPoint = obj as CanEqualColorPoint;
            if (canEqualColorPoint == null) {
                return false;
            }
            return canEqualColorPoint.CanEqual(this)
            && base.Equals(canEqualColorPoint)
            && _color == canEqualColorPoint._color;
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

