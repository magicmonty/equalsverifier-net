using System;

namespace EqualsVerifier.Tests.TestHelpers.Types
{
    public class ImmutableCanEqualPoint
    {
        readonly int _x;
        readonly int _y;

        public ImmutableCanEqualPoint(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public virtual bool CanEqual(Object obj)
        {
            return obj is ImmutableCanEqualPoint;
        }

        public override bool Equals(object obj)
        {
            var other = obj as ImmutableCanEqualPoint;
            if (other == null)
                return false;

            return other.CanEqual(this)
            && other._x == _x
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

