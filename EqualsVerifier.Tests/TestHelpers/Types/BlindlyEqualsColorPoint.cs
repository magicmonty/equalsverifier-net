using EqualsVerifier.Tests.TestHelpers;

namespace EqualsVerifier.Tests.TestHelpers.Types
{
    public sealed class BlindlyEqualsColorPoint : BlindlyEqualsPoint
    {
        readonly Color _color;

        public BlindlyEqualsColorPoint(int x, int y, Color color) : base(x, y)
        {
            _color = color;
        }

        protected override bool BlindlyEquals(object o)
        {
            var other = o as BlindlyEqualsColorPoint;
            if (other == null)
                return false;

            return base.BlindlyEquals(other)
            && other._color == _color;
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

