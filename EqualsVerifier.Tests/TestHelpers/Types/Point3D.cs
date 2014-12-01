using System;

namespace EqualsVerifier.Tests.TestHelpers.Types
{
    public class Point3D : Point
    {
        public int Z;

        public Point3D(int x, int y, int z) : base(x, y)
        {
            Z = z;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Point3D;
            if (other == null)
                return false;

            return base.Equals(obj)
            && other.Z == Z;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + (31 * Z);
        }

        public override string ToString()
        {
            return string.Format("{0},{1}", base.ToString(), Z);
        }
    }
}

