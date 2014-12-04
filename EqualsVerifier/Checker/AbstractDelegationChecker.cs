using EqualsVerifier.Util;
using System;
using System.Reflection;


namespace EqualsVerifier.Checker
{
    public class AbstractDelegationChecker<T> : AbstractChecker where T: class
    {
        readonly ClassAccessor _classAccessor;
        readonly PrefabValues _prefabValues;
        readonly Type _type;

        public AbstractDelegationChecker(ClassAccessor classAccessor)
        {
            _classAccessor = classAccessor;
            _prefabValues = _classAccessor.PrefabValues;
            _type = typeof(T);
        }

        public override void Check()
        {
            CheckAbstractEqualsAndHashCode();

            CheckAbstractDelegationInFields();

            var instance = GetRedPrefabValue<T>(_type) ?? (T)_classAccessor.GetRedObject();
            var copy = GetBlackPrefabValue<T>(_type) ?? (T)_classAccessor.GetBlackObject();

            CheckAbstractDelegation(instance, copy);

            CheckAbstractDelegationInSuper();
        }

        void CheckAbstractEqualsAndHashCode()
        {
            var equalsIsAbstract = _classAccessor.IsEqualsAbstract;
            var hashCodeIsAbstract = _classAccessor.IsGetHashCodeAbstract;

            if (equalsIsAbstract && hashCodeIsAbstract) {

                Fail(ObjectFormatter.Of(
                    "Abstract delegation: %%'s Equals and GetHashCode methods are both abstract. They should be concrete.",
                    _type.Name));
            }
            else if (equalsIsAbstract) {
                Fail(BuildSingleAbstractMethodErrorMessage(_type, true, true));
            }
            else if (hashCodeIsAbstract) {
                Fail(BuildSingleAbstractMethodErrorMessage(_type, false, true));
            }
        }

        void CheckAbstractDelegationInFields()
        {
            foreach (var field in _type.GetFields(FieldHelper.AllFields)) {
                var type = field.FieldType;
                var instance = SafelyGetInstanceOf(type);
                var copy = SafelyGetInstanceOf(type);

                if (instance != null && copy != null) {
                    CheckAbstractMethods(type, instance, copy, true);
                }
            }
        }

        void CheckAbstractDelegation(T instance, T copy)
        {
            CheckAbstractMethods(_type, instance, copy, false);
        }

        void CheckAbstractDelegationInSuper()
        {
            var superclass = _type.BaseType;
            var superAccessor = _classAccessor.GetSuperAccessor();

            bool equalsIsAbstract = superAccessor.IsEqualsAbstract;
            bool hashCodeIsAbstract = superAccessor.IsGetHashCodeAbstract;
            if (equalsIsAbstract != hashCodeIsAbstract)
                Fail(BuildSingleAbstractMethodErrorMessage(superclass, equalsIsAbstract, false));

            if (equalsIsAbstract && hashCodeIsAbstract)
                return;

            var instance = GetRedPrefabValue<object>(superclass) ?? superAccessor.GetRedObject();
            var copy = GetBlackPrefabValue<object>(_type) ?? superAccessor.GetBlackObject();
            CheckAbstractMethods(superclass, instance, copy, false);
        }

        void CheckAbstractMethods(MemberInfo instanceClass, object instance, object copy, bool prefabPossible)
        {
            try {
                instance.Equals(copy);
            }
            catch (Exception e) {
                Fail(BuildAbstractDelegationErrorMessage(instanceClass, prefabPossible, "Equals", e.Message), e);
            }

            try {
                instance.GetHashCode();
            }
            catch (Exception e) {
                Fail(BuildAbstractDelegationErrorMessage(instanceClass, prefabPossible, "GetHashCode", e.Message), e);
            }
        }

        static ObjectFormatter BuildAbstractDelegationErrorMessage(
            MemberInfo type, 
            bool prefabPossible, 
            string method, 
            string originalMessage)
        {
            var prefabFormatter = ObjectFormatter.Of("\nAdd prefab values for %%.", type.Name);
            return ObjectFormatter.Of(
                "Abstract delegation: %%'s %% method delegates to an abstract method:\n %%%%",
                type.Name, 
                method, 
                originalMessage, 
                prefabPossible ? prefabFormatter.Format() : string.Empty);
        }

        object SafelyGetInstanceOf(Type type)
        {
            var result = GetRedPrefabValue<object>(type);

            if (result != null)
                return result;

            try {
                return Instantiator.Instantiate(type);
            }
            catch {
                // If it fails for some reason, any reason, just return null.
                return null;
            }
        }

        static ObjectFormatter BuildSingleAbstractMethodErrorMessage(MemberInfo type, bool isEqualsAbstract, bool bothShouldBeConcrete)
        {
            return ObjectFormatter.Of(
                "Abstract delegation: %%'s %% method is abstract, but %% is not.\n%%",
                type.Name,
                (isEqualsAbstract ? "Equals" : "GetHashCode"),
                (isEqualsAbstract ? "GetHashCode" : "Equals"),
                (bothShouldBeConcrete ? "Both should be concrete." : "Both should be either abstract or concrete."));
        }

        TResult GetRedPrefabValue<TResult>(Type type)
        {
            return _prefabValues.Contains(type) 
                ? (TResult)_prefabValues.GetRed(type) 
                : default(TResult);

        }

        TResult GetBlackPrefabValue<TResult>(Type type)
        {
            return _prefabValues.Contains(type) 
                ? (TResult)_prefabValues.GetBlack(type) 
                : default(TResult);
        }
    }
}

