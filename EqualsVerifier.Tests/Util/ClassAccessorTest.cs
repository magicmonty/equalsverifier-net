using NUnit.Framework;
using EqualsVerifier.TestHelpers.Types;
using Shouldly;
using EqualsVerifier.TestHelpers.Annotations;

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
            _pointContainerAccessor = ClassAccessor.Of(typeof(PointContainer), _prefabValues, false);
            _abstractEqualsAndHashCodeAccessor = ClassAccessor.Of(typeof(TypeHelper.AbstractEqualsAndHashCode), _prefabValues, false);
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
            var accessor = ClassAccessor.Of(typeof(ColorPoint3D), _prefabValues, false);
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
            var accessor = ClassAccessor.Of(typeof(TypeHelper.Empty), _prefabValues, false);
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
            var accessor = ClassAccessor.Of(typeof(TypeHelper.Empty), _prefabValues, false);
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
            var accessor = ClassAccessor.Of(typeof(TypeHelper.NoFieldsSubWithFields), _prefabValues, false);
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
            var accessor = ClassAccessor.Of(typeof(ColorPoint3D), _prefabValues, false);
            var superAccessor = accessor.GetSuperAccessor();
            superAccessor.Type.ShouldBe(typeof(Point3D));
        }

        [Test]
        public void GetRedObject()
        {
            var obj = _pointContainerAccessor.GetRedObject();
            AssertObjectHasNoNullFields((PointContainer)obj);
        }

        [Test]
        public void GetRedAccessor()
        {
            var foo = _pointContainerAccessor.GetRedObject();
            var objectAccessor = _pointContainerAccessor.GetRedAccessor();
            foo.ShouldBe(objectAccessor.Get());
        }

        [Test]
        public void GetBlackObject()
        {
            var obj = _pointContainerAccessor.GetBlackObject();
            AssertObjectHasNoNullFields((PointContainer)obj);
        }

        [Test]
        public void GetBlackAccessor()
        {
            var foo = _pointContainerAccessor.GetBlackObject();
            var objectAccessor = _pointContainerAccessor.GetBlackAccessor();
            foo.ShouldBe(objectAccessor.Get());
        }

        [Test]
        public void RedAndBlackNotEqual()
        {
            var red = _pointContainerAccessor.GetRedObject();
            var black = _pointContainerAccessor.GetBlackObject();
            red.ShouldNotBe(black);
        }

        [Test]
        public void GetDefaultValuesObject()
        {
            var accessor = ClassAccessor.Of(typeof(DefaultValues), _prefabValues, false);
            var foo = (DefaultValues)accessor.GetDefaultValuesObject();
            foo.i.ShouldBe(0);
            foo.s.ShouldBe(null);
            foo.t.ShouldNotBe(null);
        }

        [Test]
        public void InstantiateAllTypes()
        {
            ClassAccessor.Of(typeof(TypeHelper.AllTypesContainer), _prefabValues, false).GetRedObject();
        }

        [Test]
        public void InstantiateArrayTypes()
        {
            ClassAccessor.Of(typeof(TypeHelper.AllArrayTypesContainer), _prefabValues, false).GetRedObject();
        }

        [Test]
        public void InstantiateRecursiveApiTypes()
        {
            ClassAccessor.Of(typeof(TypeHelper.RecursiveApiClassesContainer), _prefabValues, false).GetRedObject();
        }

        [Test]
        public void InstantiateCollectionImplementations()
        {
            ClassAccessor.Of(typeof(TypeHelper.AllRecursiveCollectionImplementationsContainer), _prefabValues, false).GetRedObject();
        }

        [Test]
        public void InstantiateInterfaceField()
        {
            ClassAccessor.Of(typeof(TypeHelper.InterfaceContainer), _prefabValues, false).GetRedObject();
        }

        [Test]
        public void InstantiateAbstractClassField()
        {
            ClassAccessor.Of(typeof(TypeHelper.AbstractClassContainer), _prefabValues, false).GetRedObject();
        }


        static void AssertObjectHasNoNullFields(PointContainer foo)
        {
            foo.ShouldNotBe(null);
            foo.Point.ShouldNotBe(null);
        }

        #pragma warning disable 649
        class DefaultValues
        {
            public int i;
            public string s;

            [NonNull] 
            public string t;
        }
        #pragma warning restore 649
    }
}

