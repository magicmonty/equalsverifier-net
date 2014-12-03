using System;
using System.Linq;
using Castle.DynamicProxy;
using Castle.Components.DictionaryAdapter.Xml;

namespace EqualsVerifier.Util
{
    public static class Instantiator
    {
        public static T Instantiate<T>()
        {
            if (typeof(T).IsAbstract || typeof(T).IsInterface)
                return InstantiateAnonymousSubclass<T>();
                
            return (T)CreateInstanceOf(typeof(T));
        }

        public static object Instantiate(Type type)
        {
            if (type.IsAbstract || type.IsInterface)
                return InstantiateAnonymousSubclass(type);

            return CreateInstanceOf(type);
        }

        public static T InstantiateAnonymousSubclass<T>()
        {
            return CreateDynamicSubclass<T>();
        }

        public static object InstantiateAnonymousSubclass(Type type)
        {
            return CreateDynamicSubclass(type);
        }

        static object CreateInstanceOf(Type type)
        {
            try {
                return Activator.CreateInstance(type);
            }
            catch (MissingMethodException) {

                var parameters = type
                    .GetConstructors()
                    .FirstOrDefault()
                    .GetParameters()
                    .Select(p => p.ParameterType)
                    .Select(t => t.IsValueType ? Activator.CreateInstance(t) : null)
                    .ToArray();

                return Activator.CreateInstance(type, parameters);
            }
        }

        static T CreateDynamicSubclass<T>()
        {
            return (T)CreateDynamicSubclass(typeof(T));
        }

        static object CreateDynamicSubclass(Type baseType)
        {
            var proxyBuilder = new DefaultProxyBuilder();
            var proxy = baseType.IsInterface
                ? proxyBuilder.CreateInterfaceProxyTypeWithoutTarget(baseType, null, ProxyGenerationOptions.Default)
                : proxyBuilder.CreateClassProxyType(baseType, null, ProxyGenerationOptions.Default);
            return CreateInstanceOf(proxy);
        }
    }
}

