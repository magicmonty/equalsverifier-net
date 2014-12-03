namespace EqualsVerifier.TestHelpers.Types
{
    public sealed class ColorPoint3D : Point3D
    {
        #pragma warning disable 414
        readonly Color _color;
        #pragma warning restore 414

        public ColorPoint3D(int x, int y, int z, Color color) : base(x, y, z)
        {
            _color = color;
        }
    }
}

