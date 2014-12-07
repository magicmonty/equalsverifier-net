using EqualsVerifier.Util;
using System.Collections.Generic;
using System;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace EqualsVerifier.Checker
{
    public class HierarchyChecker<T> : AbstractChecker
    {
        readonly Type _type;
        readonly ClassAccessor _classAccessor;
        readonly ISet<Warning> _warningsToSuppress;
        readonly bool _usingGetClass;
        readonly bool _hasRedefinedSuperclass;
        readonly Type _redefinedSubclass;
        readonly ObjectAccessor _referenceAccessor;
        readonly T _reference;
        readonly bool _typeIsSealed;

        public HierarchyChecker(
            ClassAccessor classAccessor, 
            ISet<Warning> warningsToSuppress, 
            bool usingGetClass, 
            bool hasRedefinedSuperclass, 
            Type redefinedSubclass)
        {
            _type = typeof(T);
            _classAccessor = classAccessor;
            _warningsToSuppress = new HashSet<Warning>(warningsToSuppress);
            _usingGetClass = usingGetClass;
            _hasRedefinedSuperclass = hasRedefinedSuperclass;
            _redefinedSubclass = redefinedSubclass;

            _referenceAccessor = _classAccessor.GetRedAccessor();
            _reference = _referenceAccessor == null ? default(T) : (T)_referenceAccessor.Get();
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

            if (_hasRedefinedSuperclass || _usingGetClass)
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

            if (_usingGetClass)
            {
                AssertFalse(
                    ObjectFormatter.Of(
                        "Subclass: object is equal to an instance of a trivial subclass with equal fields:\n  %%\nThis should not happen when using getClass().",
                        _reference),
                    _reference.Equals(equalSub));
            }
            else
            {
                AssertTrue(
                    ObjectFormatter.Of(
                        "Subclass: object is not equal to an instance of a trivial subclass with equal fields:\n  %%\nConsider making the class final.",
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
                    "Subclass: %% has a final equals method.\nNo need to supply a redefined subclass.",
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

            if (_usingGetClass)
            {
                AssertEquals(
                    ObjectFormatter.Of("Finality: equals and hashCode must both be sealed or both be non-sealed."),
                    equalsIsSealed, 
                    getHashCodeIsSealed);
            }
            else
            {
                AssertTrue(
                    ObjectFormatter.Of("Subclass: equals is not final.\nSupply an instance of a redefined subclass using withRedefinedSubclass if equals cannot be final."),
                    equalsIsSealed);
                AssertTrue(ObjectFormatter.Of("Subclass: hashCode is not final.\nSupply an instance of a redefined subclass using withRedefinedSubclass if hashCode cannot be final."),
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

