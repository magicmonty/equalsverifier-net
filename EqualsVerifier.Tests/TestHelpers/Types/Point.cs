namespace EqualsVerifier.TestHelpers.Types
{
    public class Point
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Point;
            if (other == null)
                return false;

            return other.X == X
            && other.Y == Y;
        }

        public override int GetHashCode()
        {
            return X + (31 * Y);
        }

        public override string ToString()
        {
            return string.Format("{0}:{1},{2}", GetType().Name, X, Y);
        }
    }
}

