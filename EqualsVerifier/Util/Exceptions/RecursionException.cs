using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace EqualsVerifier.Util.Exceptions
{
    public class RecursionException: InternalException
    {
        readonly Stack<Type> _typeStack;

        public RecursionException(Stack<Type> typeStack) : base()
        {
            _typeStack = typeStack;
        }

        public override string Message {
            get
            {
                var sb = new StringBuilder();
                sb.Append("Recursive datastructure.\nAdd prefab values for one of the following types: ");
                sb.Append(string.Join(", ", _typeStack.Select(t => t.Name)));
                sb.Append(".");
                return sb.ToString();
            }
        }
    }
}

