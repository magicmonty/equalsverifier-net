using System;
using System.Collections.Generic;
using System.Linq;

namespace EqualsVerifier.Util
{
    public class SupportedAttributes : IAttribute
    {
        /// <summary>
        /// If a class is marked [Immutable], EqualsVerifier will not
        /// complain about fields not being readonly.
        public static readonly SupportedAttributes IMMUTABLE = new SupportedAttributes(false, "Immutable");

        ///<summary>
        /// If a field is marked [NonNull] or [NotNull],
        /// <see cref="EqualsVerifier"/> will not complain about potential
        /// NullPointerExceptions being thrown if this field is null.
        /// </summary>
        public static readonly SupportedAttributes NONNULL = new SupportedAttributes(false, "Nonnull", "NonNull", "NotNull");

        public static readonly IEnumerable<IAttribute> Values = new IAttribute[] { 
            IMMUTABLE,
            NONNULL
        };

        public SupportedAttributes(bool inherits, params string[] descriptors)
        {
            _inherits = inherits;
            _descriptors = descriptors ?? Enumerable.Empty<string>();
        }

        public IEnumerable<string> Descriptors { get { return _descriptors; } }

        readonly IEnumerable<string> _descriptors;

        public bool Inherits { get { return _inherits; } }

        readonly bool _inherits;
    }
}
