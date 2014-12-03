using NUnit.Framework;
using EqualsVerifier.TestHelpers.Types;
using Shouldly;

namespace EqualsVerifier.Util
{
    [TestFixture]
    public class InstantiatorTest
    {
        [Test]
        public void InstantiateClass()
        {
            var p = Instantiator.Instantiate<Point>();
            p.ShouldBeOfType(typeof(Point));
        }

        [Test]
        public void FieldsOfInstantiatedObjectHaveDefaultValues()
        {
            var p = Instantiator.Instantiate<ColorBlindColorPoint>();
            p.X.ShouldBe(0);
            p.Color.ShouldBe(Color.UNKNOWN);
        }

        [Test]
        public void InstantiateInterface()
        {
            var i = Instantiator.Instantiate<TypeHelper.IInterface>();
            i.ShouldBeAssignableTo(typeof(TypeHelper.IInterface));
        }

        [Test]
        public void InstantiateFinalClass()
        {
            var p = Instantiator.Instantiate<SealedPoint>();

            p.ShouldBeOfType(typeof(SealedPoint));
        }

        [Test]
        public void InstantiateArrayContainer()
        {
            var c = Instantiator.Instantiate<TypeHelper.ArrayContainer>();
            c.ShouldBeOfType(typeof(TypeHelper.ArrayContainer));
            c.Field.ShouldBe(null);
        }

        [Test]
        public void InstantiateAbstractClass()
        {
            var ac = Instantiator.Instantiate<TypeHelper.AbstractClass>();
            ac.ShouldBeAssignableTo(typeof(TypeHelper.AbstractClass));
        }

        [Test]
        public void InstantiateSubclass()
        {
            var p = Instantiator.InstantiateAnonymousSubclass<Point>();
            p.ShouldNotBeOfType(typeof(Point));
            p.ShouldBeAssignableTo(typeof(Point));
        }
    }
}

