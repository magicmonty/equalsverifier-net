namespace EqualsVerifier.TestHelpers.Types
{
    public sealed class ColorBlindColorPoint : Point
    {
        public readonly Color Color;

        public ColorBlindColorPoint(int x, int y, Color color) : base(x, y)
        {
            this.Color = color;
        }

        public override string ToString()
        {
            return string.Format("{0},{1}", base.ToString(), Color);
        }
    }
}
