using System;

namespace EqualsVerifier.Tests.TestHelpers.Types
{
    public class BlindlyEqualsPoint
    {
        readonly int _x;
        readonly int _y;

        public BlindlyEqualsPoint(int x, int y)
        {
            _x = x;
            _y = y;
        }

        protected virtual bool BlindlyEquals(Object o)
        {
            var other = o as BlindlyEqualsPoint;
            if (other == null)
                return false;

            return other._x == this._x
            && other._y == this._y;
        }

        public override bool Equals(object obj)
        {
            return BlindlyEquals(obj)
            && ((BlindlyEqualsPoint)obj).BlindlyEquals(this);
        }

        public override int GetHashCode()
        {
            return _x + (31 * _y);
        }

        public override string ToString()
        {
            return string.Format("{0}: {1},{2}", GetType().Name, _x, _y);
        }
    }
}

