using System;

namespace EqualsVerifier.Tests.TestHelpers.Types
{
    public sealed class ColorPoint3D : Point3D
    {
        readonly Color _color;

        public ColorPoint3D(int x, int y, int z, Color color) : base(x, y, z)
        {
            _color = color;
        }
    }
}

