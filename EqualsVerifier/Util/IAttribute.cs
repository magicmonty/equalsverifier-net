using System.Collections.Generic;

namespace EqualsVerifier.Util
{
    public interface IAttribute
    {
        IEnumerable<string> Descriptors { get; }

        bool Inherits { get; }
    }
}
