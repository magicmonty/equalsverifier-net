using NUnit.Framework;
using System;
using Shouldly;
using EqualsVerifier.TestHelpers.Types;

namespace EqualsVerifier.Util
{
    [TestFixture]
    public class ObjectAccessorTest
    {
        [Test]
        public void Get()
        {
            var foo = new object();
            var accessor = ObjectAccessor.Of(foo);
            accessor.Get().ShouldBeSameAs(foo);
        }

        [Test]
        public void FieldAccessorFor()
        {
            var foo = new PointContainer(new Point(1, 2));
            var field = typeof(PointContainer).GetField("point", TypeHelper.DefaultBindingFlags);

            var accessor = ObjectAccessor.Of(foo);
            var fieldAccessor = accessor.FieldAccessorFor(field);

            fieldAccessor.DefaultField();
            foo.Point.ShouldBe(null);
        }
    }
}

