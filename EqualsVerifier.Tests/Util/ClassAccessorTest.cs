using NUnit.Framework;
using EqualsVerifier.TestHelpers.Types;
using Shouldly;

namespace EqualsVerifier.Util
{
    [TestFixture]
    public class ClassAccessorTest
    {
        PrefabValues _prefabValues;
        ClassAccessor _pointContainerAccessor;
        ClassAccessor _abstractEqualsAndHashCodeAccessor;

        [SetUp]
        public void Setup()
        {
            _prefabValues = new PrefabValues(new StaticFieldValueStash());
            NetApiPrefabValues.AddTo(_prefabValues);
            _pointContainerAccessor = ClassAccessor.Of(typeof(PointContainer), _prefabValues);
            _abstractEqualsAndHashCodeAccessor = ClassAccessor.Of(typeof(TypeHelper.AbstractEqualsAndHashCode), _prefabValues);
        }

        [Test]
        public void ShouldInitializeTypeCorrectly()
        {
            _pointContainerAccessor.Type.ShouldBeSameAs(typeof(PointContainer));
        }

        [Test]
        public void ShouldInitializePrefabValuesCorrectly()
        {
            _pointContainerAccessor.PrefabValues.ShouldBeSameAs(_prefabValues);
        }

        [Test]
        public void DeclaresField()
        {
            var field = typeof(PointContainer).GetField("point", TypeHelper.DefaultBindingFlags);
            _pointContainerAccessor.DeclaresField(field).ShouldBe(true);
        }

        [Test]
        public void DoesNotDeclareField()
        {
            var accessor = ClassAccessor.Of(typeof(ColorPoint3D), _prefabValues);
            var field = typeof(Point3D).GetField("z", TypeHelper.DefaultBindingFlags);
            accessor.DeclaresField(field).ShouldBe(false);
        }

        [Test]
        public void DeclaresEquals()
        {
            _pointContainerAccessor.DeclaresEquals().ShouldBe(true);
            _abstractEqualsAndHashCodeAccessor.DeclaresEquals().ShouldBe(true);
        }

        [Test]
        public void DoesNotDeclareEquals()
        {
            var accessor = ClassAccessor.Of(typeof(TypeHelper.Empty), _prefabValues);
            accessor.DeclaresEquals().ShouldBe(false);
        }

        [Test]
        public void DeclaresGetHashCode()
        {
            _pointContainerAccessor.DeclaresGetHashCode().ShouldBe(true);
            _abstractEqualsAndHashCodeAccessor.DeclaresGetHashCode().ShouldBe(true);
        }

        [Test]
        public void DoesNotDeclareGetHashCode()
        {
            var accessor = ClassAccessor.Of(typeof(TypeHelper.Empty), _prefabValues);
            accessor.DeclaresGetHashCode().ShouldBe(false);
        }

        [Test]
        public void EqualsIsNotAbstract()
        {
            _pointContainerAccessor.IsEqualsAbstract.ShouldBe(false);
        }

        [Test]
        public void EqualsIsAbstract()
        {
            _abstractEqualsAndHashCodeAccessor.IsEqualsAbstract.ShouldBe(true);
        }

        [Test]
        public void GetHashcodeIsNotAbstract()
        {
            _pointContainerAccessor.IsGetHashCodeAbstract.ShouldBe(false);
        }

        [Test]
        public void GetHashcodeIsAbstract()
        {
            _abstractEqualsAndHashCodeAccessor.IsGetHashCodeAbstract.ShouldBe(true);
        }

        [Test]
        public void EqualsIsInheritedFromObject()
        {
            var accessor = ClassAccessor.Of(typeof(TypeHelper.NoFieldsSubWithFields), _prefabValues);
            accessor.IsEqualsInheritedFromObject.ShouldBe(true);
        }

        [Test]
        public void EqualsIsNotInheritedFromObject()
        {
            _pointContainerAccessor.IsEqualsInheritedFromObject.ShouldBe(false);
        }

        [Test]
        public void GetSuperAccessorForPoco()
        {
            var superAccessor = _pointContainerAccessor.GetSuperAccessor();
            superAccessor.Type.ShouldBe(typeof(object));
        }

        [Test]
        public void GetSuperAccessorInHierarchy()
        {
            var accessor = ClassAccessor.Of(typeof(ColorPoint3D), _prefabValues);
            var superAccessor = accessor.GetSuperAccessor();
            superAccessor.Type.ShouldBe(typeof(Point3D));
        }

        [Test]
        public void GetRedObject()
        {
            Assert.Inconclusive();
            var obj = _pointContainerAccessor.GetRedObject();
            AssertObjectHasNoNullFields((PointContainer)obj);
        }

        static void AssertObjectHasNoNullFields(PointContainer foo)
        {
            foo.ShouldNotBe(null);
            foo.Point.ShouldNotBe(null);
        }
    }
}

