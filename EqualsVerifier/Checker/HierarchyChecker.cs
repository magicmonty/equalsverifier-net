using EqualsVerifier.Util;
using System.Collections.Generic;
using System;

namespace EqualsVerifier.Checker
{
    public class HierarchyChecker<T> : AbstractChecker
    {
        readonly Type _type;
        readonly ClassAccessor _classAccessor;
        readonly ISet<Warning> _warningsToSuppress;
        readonly bool _usingGetType;
        readonly bool _hasRedefinedSuperclass;
        readonly Type _redefinedSubclass;
        readonly ObjectAccessor _referenceAccessor;
        readonly T _reference;
        readonly bool _typeIsSealed;

        public HierarchyChecker(
            ClassAccessor classAccessor, 
            ISet<Warning> warningsToSuppress, 
            bool usingGetType, 
            bool hasRedefinedSuperclass, 
            Type redefinedSubclass)
        {
            _type = typeof(T);
            _classAccessor = classAccessor;
            _warningsToSuppress = new HashSet<Warning>(warningsToSuppress);
            _usingGetType = usingGetType;
            _hasRedefinedSuperclass = hasRedefinedSuperclass;
            _redefinedSubclass = redefinedSubclass;

            _referenceAccessor = _classAccessor.GetRedAccessor();
            _reference = (T)_referenceAccessor.Get();
            _typeIsSealed = _type.IsSealed;
        }

        public override void Check()
        {
            CheckSuperclass();
            CheckSubclass();
            CheckRedefinedSubclass();

            if (!_warningsToSuppress.Contains(Warning.STRICT_INHERITANCE))
                CheckSealedEqualsMethod();
        }

        void CheckSuperclass()
        {
            var superclass = _type.BaseType;
            var superAccessor = _classAccessor.GetSuperAccessor();
            if (superAccessor.IsEqualsInheritedFromObject)
                return;

            var equalSuper = ObjectAccessor.Of(_reference, superclass).Copy();

            if (_hasRedefinedSuperclass || _usingGetType)
            {
                AssertFalse(
                    ObjectFormatter.Of(
                        "Redefined superclass:\n  %%\nshould not equal superclass instance\n  %%\nbut it does.",
                        _reference,
                        equalSuper),
                    _reference.Equals(equalSuper) || equalSuper.Equals(_reference));
            }
            else
            {
                var shallow = (T)_referenceAccessor.Copy();
                ObjectAccessor.Of(shallow).ShallowScramble(_classAccessor.PrefabValues);

                AssertTrue(
                    ObjectFormatter.Of(
                        "Symmetry:\n  %%\ndoes not equal superclass instance\n  %%",
                        _reference,
                        equalSuper),
                    _reference.Equals(equalSuper) && equalSuper.Equals(_reference));

                AssertTrue(
                    ObjectFormatter.Of(
                        "Transitivity:\n  %%\nand\n  %%\nboth equal superclass instance\n  %%\nwhich implies they equal each other.",
                        _reference,
                        shallow,
                        equalSuper),
                    _reference.Equals(shallow) || _reference.Equals(equalSuper) != equalSuper.Equals(shallow));

                AssertTrue(
                    ObjectFormatter.Of(
                        "Superclass: hashCode for\n  %% (%%)\nshould be equal to hashCode for superclass instance\n  %% (%%)",
                        _reference,
                        _reference.GetHashCode(),
                        equalSuper,
                        equalSuper.GetHashCode()),
                    _reference.GetHashCode() == equalSuper.GetHashCode());
            }
        }

        void CheckSubclass()
        {
            if (_typeIsSealed)
                return;

            var equalSub = (T)_referenceAccessor.CopyIntoAnonymousSubclass();

            if (_usingGetType)
            {
                AssertFalse(
                    ObjectFormatter.Of(
                        "Subclass: object is equal to an instance of a trivial subclass with equal fields:\n  %%\nThis should not happen when using GetType().",
                        _reference),
                    _reference.Equals(equalSub));
            }
            else
            {
                AssertTrue(
                    ObjectFormatter.Of(
                        "Subclass: object is not equal to an instance of a trivial subclass with equal fields:\n  %%\nConsider making the class sealed.",
                        _reference),
                    _reference.Equals(equalSub));
            }
        }

        void CheckRedefinedSubclass()
        {
            if (_typeIsSealed || _redefinedSubclass == null)
                return;

            if (MethodIsFinal("Equals", typeof(object)))
            {
                Fail(ObjectFormatter.Of(
                    "Subclass: %% has a sealed Equals method.\nNo need to supply a redefined subclass.",
                    _type.Name));
            }

            var redefinedSub = (T)_referenceAccessor.CopyIntoSubclass(_redefinedSubclass);
            AssertFalse(
                ObjectFormatter.Of("Subclass:\n  %%\nequals subclass instance\n  %%", _reference, redefinedSub),
                _reference.Equals(redefinedSub));
        }

        void CheckSealedEqualsMethod()
        {
            if (_typeIsSealed || _redefinedSubclass != null)
                return;

            var equalsIsSealed = MethodIsFinal("Equals", typeof(object));
            var getHashCodeIsSealed = MethodIsFinal("GetHashCode");

            if (_usingGetType)
            {
                AssertEquals(
                    ObjectFormatter.Of("Finality: Equals and GetHashCode must both be sealed or both be non-sealed."),
                    equalsIsSealed, 
                    getHashCodeIsSealed);
            }
            else
            {
                AssertTrue(
                    ObjectFormatter.Of("Subclass: Equals is not sealed.\nSupply an instance of a redefined subclass using WithRedefinedSubclass if Equals cannot be sealed."),
                    equalsIsSealed);
                AssertTrue(ObjectFormatter.Of("Subclass: GetHashCode is not sealed.\nSupply an instance of a redefined subclass using WithRedefinedSubclass if GetHashCode cannot be sealed."),
                    getHashCodeIsSealed);
            }
        }

        bool MethodIsFinal(string methodName, params Type[] parameterTypes)
        {
            try
            {
                var method = _type.GetMethod(methodName, FieldHelper.AllFields, null, parameterTypes, null);
                return method.IsFinal;
            }
            catch (MethodAccessException e)
            {
                AssertionError("Security error: cannot access equals method for class " + _type.Name, e);
            }
            catch (MissingMethodException e)
            {
                AssertionError("Impossible: class " + _type.Name + " has no Equals method.", e);
            }
            return false;
        }
    }
}

