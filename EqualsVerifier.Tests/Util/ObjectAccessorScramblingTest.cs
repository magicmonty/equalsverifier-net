using NUnit.Framework;
using EqualsVerifier.TestHelpers.Types;
using Shouldly;

namespace EqualsVerifier.Util
{
    [TestFixture]
    public class ObjectAccessorScramblingTest
    {
        PrefabValues _prefabValues;

        [SetUp]
        public void SetUp()
        {
            _prefabValues = new PrefabValues(new StaticFieldValueStash());
            NetApiPrefabValues.AddTo(_prefabValues);
        }

        [Test]
        public void Scramble()
        {
            var original = new Point(2, 3);
            var copy = Copy(original);

            copy.ShouldBe(original);
            Scramble(copy);
            copy.ShouldNotBe(original);
        }

        [Test]
        public void DeepScramble()
        {
            var modified = new Point3D(2, 3, 4);
            var reference = Copy(modified);

            Scramble(modified);

            modified.ShouldNotBe(reference);
            modified.Z = 4;
            modified.ShouldNotBe(reference);
        }

        [Test]
        public void ShallowScramble()
        {
            var modified = new Point3D(2, 3, 4);
            var reference = Copy(modified);

            ObjectAccessor.Of(modified).ShallowScramble(_prefabValues);

            modified.ShouldNotBe(reference);
            modified.Z = 4;
            modified.ShouldBe(reference);
        }

        [Test]
        public void ScrambleStaticFinal()
        {
            var foo = new TypeHelper.StaticFinalContainer();
            var originalInt = TypeHelper.StaticFinalContainer.CONST;
            var originalObject = TypeHelper.StaticFinalContainer.OBJECT;

            Scramble(foo);

            TypeHelper.StaticFinalContainer.CONST.ShouldBe(originalInt);
            TypeHelper.StaticFinalContainer.OBJECT.ShouldBeSameAs(originalObject);
        }

        [Test]
        public void ScrambleString()
        {
            var foo = new StringContainer();
            var before = foo.s;
            Scramble(foo);
            foo.s.ShouldNotBe(before);
        }

        [Test]
        public void PrivateReadonlyStringCannotBeScrambled()
        {
            var foo = new FinalAssignedStringContainer();
            var before = foo.s;

            Scramble(foo);

            foo.s.ShouldBe(before);
        }

        [Test]
        public void ScramblePrivateFinalPoint()
        {
            _prefabValues.Put(typeof(Point), new Point(1, 2), new Point(2, 3));
            var foo = new FinalAssignedPointContainer();
            var before = foo.p;

            foo.p.ShouldBe(before);
            Scramble(foo);
            foo.p.ShouldNotBe(before);
        }

        static T Copy<T>(T source)
        {
            return (T)ObjectAccessor.Of(source).Copy();
        }

        void Scramble(object obj)
        {
            ObjectAccessor.Of(obj).Scramble(_prefabValues);
        }

        #pragma warning disable 414
        class StringContainer
        {
            public string s = "x";
        }

        class FinalAssignedStringContainer
        {
            public readonly string s = "x";
        }

        class FinalAssignedPointContainer
        {
            public readonly Point p = new Point(2, 3);
        }
        #pragma warning restore 414
    }
}

