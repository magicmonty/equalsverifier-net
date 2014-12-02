using System;
using Castle.DynamicProxy;

namespace EqualsVerifier.Util
{
    public class Instantiator<T>
    {
        public static Instantiator<T> Of<T>()
        {
            return new Instantiator<T>();
        }

        public T Instantiate()
        {
            if (typeof(T).IsAbstract || typeof(T).IsInterface)
                return (T)CreateDynamicSubclass(typeof(T));

            return Activator.CreateInstance<T>();
        }

        public T InstantiateAnonymousSubclass()
        {
            return (T)CreateDynamicSubclass(typeof(T));
        }

        static object CreateDynamicSubclass(Type superclass)
        {
            var proxyBuilder = new DefaultProxyBuilder();
            var proxy = proxyBuilder.CreateClassProxyType(superclass, null, ProxyGenerationOptions.Default);
            return Activator.CreateInstance(proxy);
        }
    }
}

