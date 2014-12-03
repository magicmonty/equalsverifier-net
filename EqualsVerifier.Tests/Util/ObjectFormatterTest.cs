using NUnit.Framework;
using Shouldly;
using System;

namespace EqualsVerifier.Util
{
    [TestFixture]
    public class ObjectFormatterTest
    {
        [Test]
        public void NoParameters()
        {
            var sut = ObjectFormatter.Of("No parameters");
            sut.Format().ShouldBe("No parameters");
        }

        [Test]
        public void OneSimpleParameter()
        {
            var sut = ObjectFormatter.Of("One simple parameter: %%", new Simple(42));
            sut.Format().ShouldBe("One simple parameter: Simple: 42");
        }

        [Test]
        public void MultipleSimpleParameters()
        {
            var sut = ObjectFormatter.Of("Multiple simple parameters: %% and %% and also %%", new Simple(0), new Simple(1), new Simple(2));
            sut.Format().ShouldBe("Multiple simple parameters: Simple: 0 and Simple: 1 and also Simple: 2");
        }

        [Test]
        public void OneThrowingParameter()
        {
            var sut = ObjectFormatter.Of("One throwing parameter: %%", new Throwing(1337, "string"));
            sut.Format().ShouldBe("One throwing parameter: [Throwing _i=1337 _s=string]-throws ArgumentException(msg)");
        }

        [Test]
        public void OneThrowingParameterWithNullSubparameter()
        {
            var sut = ObjectFormatter.Of("One throwing parameter: %%", new Throwing(1337, null));
            sut.Format().ShouldBe("One throwing parameter: [Throwing _i=1337 _s=null]-throws ArgumentException(msg)");
        }

        [Test]
        public void OneParameterWithNoFieldsAndThrowsWithNullMessage()
        {
            var sut = ObjectFormatter.Of("No fields, null message: %%", new NoFieldsAndThrowsNullMessage());
            sut.Format().ShouldBe("No fields, null message: [NoFieldsAndThrowsNullMessage]-throws ArgumentNullException(Argument cannot be null.)");
        }

        [Test]
        public void OneAbstractParameter()
        {
            var sut = ObjectFormatter.Of("Abstract: %%", Instantiator.Instantiate<Abstract>());
            sut.Format().ShouldContain("Abstract: [Abstract x=10]-throws NotImplementedException()");
        }

        [Test]
        public void OneConcreteSubclassParameter()
        {
            var sut = ObjectFormatter.Of("Concrete: %%", Instantiator.Instantiate<AbstractImpl>());
            sut.Format().ShouldContain("Concrete: something concrete");
        }

        [Test]
        public void OneDelegatedAbstractParameter()
        {
            var sut = ObjectFormatter.Of("Abstract: %%", Instantiator.Instantiate<AbstractDelegation>());
            sut.Format().ShouldContain("Abstract: [AbstractDelegation y=20]-throws NotImplementedException()");
        }

        [Test]
        public void OneDelegatedConcreteSubclassParameter()
        {
            var sut = ObjectFormatter.Of("Concrete: %%", Instantiator.Instantiate<AbstractDelegationImpl>());
            sut.Format().ShouldContain("Concrete: something concrete");
        }

        [Test]
        public void OneThrowingContainerParameter()
        {
            var tc = new ThrowingContainer(Instantiator.Instantiate<Throwing>());
            var sut = ObjectFormatter.Of("TC: %%", tc);
            sut.Format().ShouldContain("TC: [ThrowingContainer _t=[Throwing _i=0 _s=null]-throws ArgumentException(msg)]-throws ArgumentException(msg)");
        }

        [Test]
        public void OneAbstractContainerParameter()
        {
            var ac = new AbstractContainer(Instantiator.Instantiate<AbstractDelegation>());

            var sut = ObjectFormatter.Of("AC: %%", ac);
            sut.Format().ShouldContain("AC: [AbstractContainer _ad=[AbstractDelegation y=20]-throws NotImplementedException()]-throws NotImplementedException()");
        }

        [Test]
        public void ParameterWithMixOfVariousFields()
        {
            var mix = new Mix();
            mix.Throwing = new Throwing(42, "empty");

            var sut = ObjectFormatter.Of("%%", mix);
            sut.Format().ShouldBe("[Mix _i=42 _s=null _t=not null Throwing=[Throwing _i=42 _s=empty]-throws ArgumentException(msg)]-throws InvalidOperationException(Operation is not valid due to the current state of the object)");
        }

        [Test]
        public void ConnectedParameters()
        {
            var sut = ObjectFormatter.Of("%%%%", 1, 2);
            sut.Format().ShouldContain("12");
        }

        [Test]
        public void NullParameter()
        {
            var sut = ObjectFormatter.Of("This parameter is null: %%", (Object)null);
            sut.Format().ShouldBe("This parameter is null: null");
        }

        [Test]
        public void NullMessage()
        {
            Should.Throw<ArgumentNullException>(() => {
                ObjectFormatter.Of(null);
            });
        }

        [Test]
        public void NotEnoughParameters()
        {
            var sut = ObjectFormatter.Of("Not enough: %% and %%");

            Should.Throw<InvalidOperationException>(() => {
                sut.Format();
            });

            try {
                sut.Format();
            }
            catch (InvalidOperationException e) {
                e.Message.ShouldBe("Not enough parameters");
            }

        }

        [Test]
        public void TooManyParameters()
        {
            var sut = ObjectFormatter.Of("Too many!", new Simple(0));

            Should.Throw<InvalidOperationException>(() => {
                sut.Format();
            });

            try {
                sut.Format();
            }
            catch (InvalidOperationException e) {
                e.Message.ShouldBe("Too many parameters");
            }
        }

        #pragma warning disable 414
        sealed class Simple
        {
            readonly int _i;

            public Simple(int i)
            {
                _i = i;
            }

            public override string ToString()
            {
                return string.Format("Simple: {0}", _i);
            }
        }

        sealed class Throwing
        {
            readonly int _i;
            readonly string _s;

            public Throwing(int i, string s)
            {
                _i = i;
                _s = s;
            }

            public override string ToString()
            {
                throw new ArgumentException("msg");
            }
        }

        sealed class NoFieldsAndThrowsNullMessage
        {
            public override string ToString()
            {
                throw new ArgumentNullException();
            }
        }

        public abstract class Abstract
        {
            readonly int x = 10;

            public override abstract string ToString();
        }

        sealed class AbstractImpl : Abstract
        {
            public override string ToString()
            {
                return "something concrete";
            }
        }

        public abstract class AbstractDelegation
        {
            readonly int y = 20;

            public abstract string SomethingAbstract();

            public override string ToString()
            {
                return SomethingAbstract();
            }
        }

        sealed class AbstractDelegationImpl : AbstractDelegation
        {
            public override string SomethingAbstract()
            {
                return "something concrete";
            }
        }

        sealed class ThrowingContainer
        {
            private readonly Throwing _t;

            public ThrowingContainer(Throwing t)
            {
                _t = t;
            }

            public override string ToString()
            {
                return "ThrowingContainer " + _t.ToString();
            }
        }

        sealed class AbstractContainer
        {
            public readonly AbstractDelegation _ad;

            public AbstractContainer(AbstractDelegation ad)
            {
                _ad = ad;
            }

            public override string ToString()
            {
                return "AbstractContainer: " + _ad.ToString();
            }
        }

        sealed class Mix
        {
            public readonly int _i = 42;
            public readonly string _s = null;
            public readonly string _t = "not null";
            public Throwing Throwing;

            public override String ToString()
            {
                throw new InvalidOperationException();
            }
        }
        #pragma warning restore 414
    }
}

