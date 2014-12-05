using EqualsVerifier.TestHelpers;
using NUnit.Framework;
using System;

namespace EqualsVerifier.Integration.ExtendedContract
{
    public class AbstractDelegationTest : IntegrationTestBase
    {
        const string ABSTRACT_DELEGATION = "Abstract delegation";
        const string EQUALS_DELEGATES = "equals method delegates to an abstract method";
        const string HASHCODE_DELEGATES = "hashCode method delegates to an abstract method";
        const string PREFAB = "Add prefab values for";

        [Test]
        public void WhenClassHasAFieldOfAnAbstractClass_ThenSucceed()
        {
            EqualsVerifier
                .ForType<AbstractContainer>()
                .Verify();
        }

        [Test]
        public void WhenEqualsCallsAnAbstractMethod_ThenFailGracefully()
        {
            ExpectFailureWithCause<NullReferenceException>(
                () => EqualsVerifier.ForType<AbstractEqualsDelegator>().Verify(),
                ABSTRACT_DELEGATION, 
                EQUALS_DELEGATES, 
                typeof(AbstractEqualsDelegator).Name);
        }

        [Test]
        public void WhenHashCodeCallsAnAbstractMethod_ThenFailGracefully()
        {
            ExpectFailureWithCause<NullReferenceException>(
                () => EqualsVerifier.ForType<AbstractHashCodeDelegator>().Verify(),
                ABSTRACT_DELEGATION, 
                HASHCODE_DELEGATES, 
                typeof(AbstractHashCodeDelegator).Name);
        }

        [Test]
        public void WhenTostringCallsAnAbstractMethod_ThenSucceed()
        {
            EqualsVerifier
                .ForType<AbstractTostringDelegator>()
                .Verify();
        }

        [Test]
        public void WhenEqualsCallsAnAbstractFieldsAbstractMethod_ThenFailGracefully()
        {
            ExpectFailureWithCause<NotImplementedException>(
                () => EqualsVerifier.ForType<EqualsDelegatesToAbstractMethodInField>().Verify(),
                ABSTRACT_DELEGATION, 
                EQUALS_DELEGATES, 
                typeof(EqualsDelegatesToAbstractMethodInField).Name);
        }

        [Test]
        public void GivenAConcretePrefabImplementationOfSaidAbstractField_WhenEqualsCallsAnAbstractFieldsAbstactMethod_ThenSucceed()
        {
            EqualsVerifier
                .ForType<EqualsDelegatesToAbstractMethodInField>()
                .WithPrefabValues<AbstractDelegator>(new AbstractDelegatorImpl(), new AbstractDelegatorImpl())
                .Verify();
        }

        [Test]
        public void WhenHashCodeCallsAnAbstractFieldsAbstactMethod_ThenFailGracefully()
        {
            ExpectFailureWithCause<NotImplementedException>(
                () => EqualsVerifier.ForType<HashCodeDelegatesToAbstractMethodInField>().Verify(),
                ABSTRACT_DELEGATION, 
                HASHCODE_DELEGATES, 
                typeof(HashCodeDelegatesToAbstractMethodInField).Name);
        }

        [Test]
        public void GivenAConcretePrefabImplementationOfSaidAbstractField_WhenHashCodeCallsAnAbstractFieldsAbstactMethod_ThenSucceed()
        {
            EqualsVerifier
                .ForType<HashCodeDelegatesToAbstractMethodInField>()
                .WithPrefabValues<AbstractDelegator>(new AbstractDelegatorImpl(), new AbstractDelegatorImpl())
                .Verify();
        }

        [Test]
        public void WhenTostringCallsAnAbstractFieldsAbstractMethod_ThenSucceed()
        {
            EqualsVerifier
                .ForType<TostringDelegatesToAbstractMethodInField>()
                .Verify();
        }

        [Test]
        public void WhenAFieldsEqualsMethodCallsAnAbstractField_ThenFailGracefully()
        {
            ExpectFailureWithCause<NullReferenceException>(
                () => EqualsVerifier.ForType<EqualsInFieldDelegatesToAbstractMethod>().Verify(),
                ABSTRACT_DELEGATION, 
                EQUALS_DELEGATES, 
                PREFAB, 
                typeof(AbstractEqualsDelegator).Name);
        }

        [Test]
        public void GivenAConcretePrefabImplementationOfSaidField_WhenAFieldsEqualsMethodCallsAnAbstractField_ThenDucceed()
        {
            EqualsVerifier
                .ForType<EqualsInFieldDelegatesToAbstractMethod>()
                .WithPrefabValues<AbstractEqualsDelegator>(
                new AbstractEqualsDelegatorImpl(1),
                new AbstractEqualsDelegatorImpl(2))
                .Verify();
        }

        [Test]
        public void WhenAFieldsHashCodeMethodCallsAnAbstractField_ThenFailGracefully()
        {
            ExpectFailureWithCause<NullReferenceException>(
                () => EqualsVerifier.ForType<HashCodeInFieldDelegatesToAbstractMethod>().Verify(),
                ABSTRACT_DELEGATION, 
                HASHCODE_DELEGATES, 
                PREFAB, 
                typeof(AbstractHashCodeDelegator).Name);
        }

        [Test]
        public void GivenAConcretePrefabImplementationOfSaidField_WhenAFieldsHashCodeMethodCallsAnAbstractField_ThenSucceed()
        {
            EqualsVerifier
                .ForType<HashCodeInFieldDelegatesToAbstractMethod>()
                .WithPrefabValues<AbstractHashCodeDelegator>(
                new AbstractHashCodeDelegatorImpl(1),
                new AbstractHashCodeDelegatorImpl(2))
                .Verify();
        }

        [Test]
        public void WhenAFieldsTostringMethodCallsAnAbstractField_ThenSucceed()
        {
            EqualsVerifier
                .ForType<TostringInFieldDelegatesToAbstractMethod>()
                .Verify();
        }

        [Test]
        public void WhenEqualsInSuperclassCallsAnAbstractMethodEvenThoughItsImplementedHere_ThenFailGracefully()
        {
            ExpectFailureWithCause<NullReferenceException>(
                () => EqualsVerifier.ForType<AbstractEqualsDelegatorImpl>().Verify(),
                ABSTRACT_DELEGATION, 
                EQUALS_DELEGATES, 
                typeof(AbstractEqualsDelegator).Name);
        }

        [Test]
        public void WhenHashCodeInSuperclassCallsAnAbstractMethodEvenThoughItsImplementedHere_ThenFailGracefully()
        {
            ExpectFailureWithCause<NullReferenceException>(
                () => EqualsVerifier.ForType<AbstractHashCodeDelegatorImpl>().Verify(),
                ABSTRACT_DELEGATION, 
                HASHCODE_DELEGATES, 
                typeof(AbstractHashCodeDelegator).Name);
        }

        [Test]
        public void WhenTostringInSuperclassCallsAnAbstractMethod_ThenSucceed()
        {
            EqualsVerifier
                .ForType<AbstractToStringDelegatorImpl>()
                .Verify();
        }

        [Test]
        public void WhenEqualsVerifierSignalsAnAbstractDelegationIssue_ThenOriginalMessageIsIncludedInErrorMessage()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<ThrowsNotImplementedExceptionWithMessage>().Verify(),
                "This is NotImplementedException's original message");
        }

        internal abstract class AbstractClass
        {
            int I;

            public abstract void SomeMethod();

            public override bool Equals(object obj)
            {
                var other = obj as AbstractClass;
                return other != null && I == other.I;
            }

            public override int GetHashCode()
            { 
                return this.GetDefaultHashCode(); 
            }
        }

        internal sealed class AbstractContainer
        {
            readonly AbstractClass _foo;

            public AbstractContainer(AbstractClass ac)
            {
                _foo = ac;
            }

            public override bool Equals(object obj)
            {
                var other = obj as AbstractContainer;
                return other != null
                && _foo.NullSafeEquals(other._foo);
            }

            public override int GetHashCode()
            { 
                return this.GetDefaultHashCode(); 
            }
        }

        internal abstract class AbstractEqualsDelegator
        {
            readonly int _i;

            public AbstractEqualsDelegator(int i)
            {
                _i = i;
            }

            public abstract bool TheAnswer();

            public override bool Equals(object obj)
            {
                if (this == obj)
                    return true;

                var other = obj as AbstractEqualsDelegator;
                return other != null && (
                    TheAnswer() || _i == other._i);
            }

            public override int GetHashCode()
            { 
                return this.GetDefaultHashCode(); 
            }
        }

        internal class AbstractEqualsDelegatorImpl : AbstractEqualsDelegator
        {
            public AbstractEqualsDelegatorImpl(int i) : base(i)
            {
            }

            public override bool TheAnswer()
            {
                return false;
            }
        }

        internal abstract class AbstractHashCodeDelegator
        {
            readonly int _i;

            public AbstractHashCodeDelegator(int i)
            {
                _i = i;
            }

            public abstract int TheAnswer();

            public override bool Equals(object obj)
            {
                return this.DefaultEquals(obj); 
            }

            public override int GetHashCode()
            {
                return _i + TheAnswer();
            }
        }

        internal class AbstractHashCodeDelegatorImpl : AbstractHashCodeDelegator
        {
            public AbstractHashCodeDelegatorImpl(int i) : base(i)
            {
            }

            public override int TheAnswer()
            {
                return 0;
            }
        }

        internal abstract class AbstractTostringDelegator
        {
            readonly int _i;

            public AbstractTostringDelegator(int i)
            {
                _i = i;
            }

            public abstract int TheAnswer();

            public sealed override bool Equals(object obj)
            {
                var other = obj as AbstractTostringDelegator;
                return other != null && _i == other._i;
            }

            public sealed override int GetHashCode()
            { 
                return this.GetDefaultHashCode(); 
            }

            public override string ToString()
            {
                return "" + TheAnswer();
            }
        }

        internal class AbstractToStringDelegatorImpl : AbstractTostringDelegator
        {
            public AbstractToStringDelegatorImpl(int i) : base(i)
            {
            }

            public override int TheAnswer()
            {
                return 0;
            }
        }

        internal abstract class AbstractDelegator
        {
            public abstract void AbstractDelegation();
        }

        internal sealed class AbstractDelegatorImpl : AbstractDelegator
        {
            public override void AbstractDelegation()
            {
            }
        }

        internal sealed class EqualsDelegatesToAbstractMethodInField
        {
            internal readonly AbstractDelegator Delegator;
            internal readonly int I;

            public EqualsDelegatesToAbstractMethodInField(AbstractDelegator ad, int i)
            {
                Delegator = ad;
                I = i;
            }

            public override bool Equals(object obj)
            {
                var other = obj as EqualsDelegatesToAbstractMethodInField;
                if (other == null)
                    return false;

                if (Delegator != null)
                    Delegator.AbstractDelegation();

                return I == other.I
                && Delegator.NullSafeEquals(other.Delegator);
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        internal sealed class HashCodeDelegatesToAbstractMethodInField
        {
            internal readonly AbstractDelegator Delegator;
            internal readonly int I;

            public HashCodeDelegatesToAbstractMethodInField(AbstractDelegator ad, int i)
            {
                Delegator = ad;
                I = i;
            }

            public override bool Equals(object obj)
            { 
                return this.DefaultEquals(obj);
            }

            public override int GetHashCode()
            {
                if (Delegator != null)
                    Delegator.AbstractDelegation();

                return this.GetDefaultHashCode();
            }
        }

        internal sealed class TostringDelegatesToAbstractMethodInField
        {
            internal readonly AbstractDelegator Delegator;
            internal readonly int I;

            public TostringDelegatesToAbstractMethodInField(AbstractDelegator ad, int i)
            {
                Delegator = ad;
                I = i;
            }

            public override bool Equals(object obj)
            {
                return this.DefaultEquals(obj);
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }

            public override string ToString()
            {
                if (Delegator != null)
                    Delegator.AbstractDelegation();
                return "..." + I;
            }
        }

        internal sealed class EqualsInFieldDelegatesToAbstractMethod
        {
            internal readonly AbstractEqualsDelegator Delegator;

            public EqualsInFieldDelegatesToAbstractMethod(AbstractEqualsDelegator aed)
            {
                Delegator = aed;
            }

            public override bool Equals(object obj)
            {
                var other = obj as EqualsInFieldDelegatesToAbstractMethod;
                return other != null && Delegator.NullSafeEquals(other.Delegator);
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }

        internal sealed class HashCodeInFieldDelegatesToAbstractMethod
        {
            internal readonly AbstractHashCodeDelegator _delegator;

            public HashCodeInFieldDelegatesToAbstractMethod(AbstractHashCodeDelegator ahcd)
            {
                _delegator = ahcd;
            }

            public override bool Equals(object obj)
            {
                return this.DefaultEquals(obj);
            }

            public override int GetHashCode()
            {
                return _delegator.GetNullSafeHashCode();
            }
        }

        internal sealed class TostringInFieldDelegatesToAbstractMethod
        {
            internal readonly AbstractTostringDelegator _delegator;

            public TostringInFieldDelegatesToAbstractMethod(AbstractTostringDelegator atsd)
            {
                _delegator = atsd;
            }

            public override bool Equals(object obj)
            {
                return this.DefaultEquals(obj);
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }

            public override string ToString()
            {
                return "..." + (_delegator == null ? string.Empty : _delegator.ToString());
            }
        }

        internal abstract class ThrowsNotImplementedExceptionWithMessage
        {
            #pragma warning disable 414 
            readonly int _i;
            #pragma warning restore 414 

            public ThrowsNotImplementedExceptionWithMessage(int i)
            {
                _i = i;
            }

            public override bool Equals(object obj)
            {
                throw new NotImplementedException(
                    "This is NotImplementedException's original message");
            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }
    }
}