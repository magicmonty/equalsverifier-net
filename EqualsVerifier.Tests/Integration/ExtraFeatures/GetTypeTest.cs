using System;
using EqualsVerifier.TestHelpers;
using NUnit.Framework;
using EqualsVerifier.TestHelpers.Types;

namespace EqualsVerifier.Integration.ExtraFeatures
{
    [TestFixture]
    public class GetTypeTest : IntegrationTestBase
    {
        [Test]
        public void GivenUsingGetTypeIsUsed_WhenEqualsUsesGetTypeInsteadOfInstanceof_ThenSucceed()
        {
            EqualsVerifier
                .ForType<GetTypePointHappyPath>()
                .UsingGetType()
                .Verify();
        }

        [Test]
        public void GivenUsingGetTypeIsUsed_WhenEqualsUsesGetTypeButForgetsToCheckNull_ThenFail()
        {
            ExpectFailureWithCause<NullReferenceException>(
                () => EqualsVerifier.ForType<GetTypePointNull>().UsingGetType().Verify(),
                "Non-nullity: NullReferenceException thrown");
        }

        [Test]
        public void GivenUsingGetTypeIsUsed_WhenEqualsUsesInstanceof_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<SealedMethodsPoint>().UsingGetType().Verify(),
                "Subclass", 
                "object is equal to an instance of a trivial subclass with equal fields:", 
                "This should not happen when using GetType().");
        }

        [Test]
        public void GivenUsingGetTypeIsUsed_WhenSuperclassUsesGetType_ThenSucceed()
        {
            EqualsVerifier
                .ForType<GetTypeColorPoint>()
                .UsingGetType()
                .Verify();
        }

        [Test]
        public void GivenUsingGetTypeIsUsed_WhenEqualsUsesGetTypeButSuperclassUsesInstanceof_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<GetTypeColorPointWithEqualSuper>().UsingGetType().Verify(),
                "Redefined superclass", 
                typeof(GetTypeColorPointWithEqualSuper).Name,
                "should not equal superclass instance", 
                typeof(Point).Name, 
                "but it does");
        }

        public class GetTypePointHappyPath
        {
            readonly int _x;
            readonly int _y;

            public GetTypePointHappyPath(int x, int y)
            {
                _x = x;
                _y = y;
            }

            public override bool Equals(object obj)
            {
                if (obj == null || obj.GetType() != GetType())
                    return false;

                var p = (GetTypePointHappyPath)obj;
                return p._x == _x && p._y == _y;
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        public class GetTypePointNull
        {
            readonly int _x;
            readonly int _y;

            public GetTypePointNull(int x, int y)
            {
                this._x = x;
                this._y = y;
            }

            public override bool Equals(object obj)
            {
                if (obj.GetType() != GetType())
                    return false;

                var p = (GetTypePointNull)obj;
                return p._x == _x && p._y == _y;
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        public class GetTypeColorPoint : GetTypePointHappyPath
        {
            readonly Color _color;

            public GetTypeColorPoint(int x, int y, Color color) : base(x, y)
            {
                _color = color;
            }

            public override bool Equals(object obj)
            {
                if (base.Equals(obj))
                {
                    var other = (GetTypeColorPoint)obj;
                    return _color == other._color;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        public class GetTypeColorPointWithEqualSuper : Point
        {
            readonly Color _color;

            public GetTypeColorPointWithEqualSuper(int x, int y, Color color) : base(x, y)
            {
                _color = color;
            }

            public override bool Equals(object obj)
            {
                if (obj == null || obj.GetType() != GetType())
                    return false;

                var other = obj as GetTypeColorPointWithEqualSuper;
                return base.Equals(obj) && _color == other._color;
            }

            public override int GetHashCode()
            { 
                return this.GetDefaultHashCode(); 
            }
        }
    }
}

