namespace EqualsVerifier.Tests.TestHelpers.Types
{
    public sealed class ColorBlindColorPoint : Point
    {
        public readonly Color Color;

        public ColorBlindColorPoint(int x, int y, Color color) : base(x, y)
        {
            this.Color = color;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0},{1}", base.ToString(), Color);
        }
    }
}
