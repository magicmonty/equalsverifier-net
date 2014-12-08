using System;
using System.Linq;
using Castle.DynamicProxy;
using System.Reflection;

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
            try
            {
                return Activator.CreateInstance(type);
            }
            catch (MissingMethodException)
            {
                var parameterTypes = type
                    .GetConstructors()
                    .FirstOrDefault()
                    .GetParameters()
                    .Select(p => p.ParameterType)
                    .ToList();

                var parameters = parameterTypes
                    .Select(t => t.IsValueType ? Activator.CreateInstance(t) : null)
                    .ToArray();

                try
                {
                    return Activator.CreateInstance(type, parameters);
                }
                catch (TargetInvocationException e)
                {
                    if (!(e.InnerException is NullReferenceException))
                        throw e;

                    parameters = parameterTypes
                        .Select(t => t.IsValueType ? Activator.CreateInstance(t) : Instantiator.Instantiate(t))
                        .ToArray();

                    return Activator.CreateInstance(type, parameters);
                }
            }
        }

        static T CreateDynamicSubclass<T>()
        {
            return (T)CreateDynamicSubclass(typeof(T));
        }

        static object CreateDynamicSubclass(Type baseType)
        {
            // Don't create sub types of already created proxy types
            if (baseType.FullName.StartsWith("Castle", StringComparison.Ordinal) && baseType.Name.Contains("Proxy"))
                baseType = baseType.BaseType;

            var proxyBuilder = new DefaultProxyBuilder();
            var proxy = baseType.IsInterface
                ? proxyBuilder.CreateInterfaceProxyTypeWithoutTarget(baseType, null, ProxyGenerationOptions.Default)
                : proxyBuilder.CreateClassProxyType(baseType, null, ProxyGenerationOptions.Default);
            return CreateInstanceOf(proxy);
        }
    }
}

