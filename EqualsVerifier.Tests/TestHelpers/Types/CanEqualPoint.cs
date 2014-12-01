using System;

namespace EqualsVerifier.Tests.TestHelpers.Types
{
    public class CanEqualPoint
    {
        private readonly int _x;
        private readonly int _y;

        public CanEqualPoint(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public virtual bool CanEqual(Object obj)
        {
            return obj is CanEqualPoint;
        }

        public override bool Equals(object obj)
        {
            var canEqualPoint = obj as CanEqualPoint;
            if (canEqualPoint == null) {
                return false;
            }
            return canEqualPoint.CanEqual(this)
            && canEqualPoint._x == _x
            && canEqualPoint._y == _y;
        }

        public override int GetHashCode()
        {
            return _x + (31 * _y);
        }

        public override string ToString()
        {
            return GetType().Name + ":" + _x + "," + _y;
        }
    }
}

