using NUnit.Framework;
using EqualsVerifier.TestHelpers.Types;
using Shouldly;
using System;
using System.Reflection;

namespace EqualsVerifier.Util
{
    [TestFixture]
    public class ObjectAccessorCopyingTest
    {
        [Test]
        public void CopyHappyPath()
        {
            var original = new Point(2, 3);
            var copy = CopyOf(original);

            AssertAllFieldsEqual(original, copy, typeof(Point));
        }

        [Test]
        public void ShallowCopy()
        {
            var original = new PointContainer(new Point(1, 2));
            var copy = CopyOf(original);

            copy.ShouldNotBeSameAs(original);
            copy.Point.ShouldBeSameAs(original.Point);
        }

        [Test]
        public void CopyStaticReadonly()
        {
            var foo = new TypeHelper.StaticFinalContainer();
            var copy = CopyOf(foo);
            AssertAllFieldsEqual(foo, copy, typeof(TypeHelper.StaticFinalContainer));
        }

        [Test]
        public void InheritanceCopy()
        {
            var original = new Point3D(2, 3, 4);
            var copy = CopyOf(original);

            AssertAllFieldsEqual(original, copy, typeof(Point));
            AssertAllFieldsEqual(original, copy, typeof(Point3D));
        }

        [Test]
        public void CopyFromSub()
        {
            var original = new Point3D(2, 3, 4);
            var copy = CopyOfSub<Point>(original, typeof(Point));

            copy.ShouldBeOfType(typeof(Point));
            AssertAllFieldsEqual(original, copy, typeof(Point));
        }

        [Test]
        public void CopyToSub()
        {
            var original = new Point(2, 3);
            Point3D copy = CopyIntoSubclass<Point, Point3D>(original);

            AssertAllFieldsEqual(original, copy, typeof(Point));
            copy.Z.ShouldBe(0);
        }

        [Test]
        public void ShallowCopyToSub()
        {
            var original = new PointContainer(new Point(1, 2));
            var copy = CopyIntoSubclass<PointContainer, SubPointContainer>(original);

            copy.ShouldNotBeSameAs(original);
            copy.Point.ShouldBeSameAs(original.Point);
        }

        [Test]
        public void InheritanceCopyToSub()
        {
            var original = new Point3D(2, 3, 4);
            var copy = CopyIntoSubclass<Point3D, ColorPoint3D>(original);

            AssertAllFieldsEqual(original, copy, typeof(Point));
            AssertAllFieldsEqual(original, copy, typeof(Point3D));
        }

        [Test]
        public void CopyToAnonymousSub()
        {
            var original = new Point(2, 3);
            var accessor = ObjectAccessor.Of(original);
            var copy = accessor.CopyIntoAnonymousSubclass();

            AssertAllFieldsEqual(original, copy, typeof(Point));
            copy.GetType().ShouldNotBe(original.GetType());
            original.GetType().IsAssignableFrom(copy.GetType()).ShouldBe(true);
        }

        static T CopyOf<T>(T original)
        {
            var accessor = ObjectAccessor.Of(original);
            return (T)accessor.Copy();
        }

        static T CopyOfSub<T>(T original, Type type)
        {
            return (T)ObjectAccessor.Of(original, type).Copy();
        }

        static TResult CopyIntoSubclass<T, TResult>(T original)
        {
            return (TResult)ObjectAccessor.Of(original).CopyIntoSubclass(typeof(TResult));
        }

        static void AssertAllFieldsEqual(object original, object copy, Type type)
        {
            copy.ShouldNotBeSameAs(original);
            foreach (var field in FieldEnumerable.Of(type))
            {
                try
                {
                    TestFrameworkBridge.AssertEquals(
                        ObjectFormatter.Of("On field: %%", field.Name), 
                        field.GetValue(copy),
                        field.GetValue(original));
                }
                catch (AssertionException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException("Error", e);
                }
            }
        }

        class SubPointContainer: PointContainer
        {
            public SubPointContainer(Point point) : base(point)
            {

            }
        }
    }
}

