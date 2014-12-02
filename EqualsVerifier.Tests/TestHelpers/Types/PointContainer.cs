namespace EqualsVerifier.TestHelpers.Types
{
    public class PointContainer
    {
        readonly Point point;

        public PointContainer(Point point)
        {
            this.point = point;
        }

        public Point Point { get { return point; } }

        public override bool Equals(object obj)
        {
            var other = obj as PointContainer;
            return other != null && point.NullSafeEquals(other.point);
        }

        public override int GetHashCode()
        {
            return point.GetNullSafeHashCode();
        }
    }
}
