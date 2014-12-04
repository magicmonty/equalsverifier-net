using System;
using System.Reflection;
using EqualsVerifier.Util;
using System.Collections.Generic;
using System.Linq;

namespace EqualsVerifier.Checker
{
    public class SignatureChecker<T> : AbstractChecker where T: class
    {
        readonly Type _type;

        public SignatureChecker()
        {
            _type = typeof(T);            
        }

        public override void Check()
        {
            var equalsMethods = GetEqualsMethods();
            if (equalsMethods.Length > 1) {
                Fail("More than one equals method found");
            }

            if (equalsMethods.Length == 0)
                return;

            CheckEquals(equalsMethods.First());
        }

        MethodInfo[] GetEqualsMethods()
        {
            var result = new List<MethodInfo>();

            foreach (var method in _type.GetMethods(FieldHelper.DeclaredOnly)) {
                if (method.Name.Equals("Equals", StringComparison.InvariantCulture)) {
                    result.Add(method);
                }
            }

            return result.ToArray();
        }

        void CheckEquals(MethodBase equals)
        {
            var parameterTypes = equals.GetParameters();

            if (parameterTypes.Length > 1) {
                Fail("Too many parameters");
            }

            if (parameterTypes.Length == 0) {
                Fail("No parameter");
            }

            var parameterType = parameterTypes[0].ParameterType;
            if (parameterType == _type) {
                Fail("Parameter should be an Object, not " + _type.Name);
            }

            if (parameterType != typeof(object)) {
                Fail("Parameter should be of type 'object'");
            }
        }

        void Fail(string message)
        {
            Fail(ObjectFormatter.Of("Overloaded: %%.\nSignature should be: public boolean equals(Object obj)", message));
        }
    }
}

