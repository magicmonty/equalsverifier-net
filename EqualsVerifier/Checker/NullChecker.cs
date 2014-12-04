using EqualsVerifier.Util;
using System.Collections.Generic;

namespace EqualsVerifier.Checker
{
    public class NullChecker<T> : IChecker
    {
        readonly ClassAccessor _classAccessor;
        readonly ISet<Warning> _warningsToSuppress;

        public NullChecker(ClassAccessor classAccessor, ISet<Warning> warningsToSuppress)
        {
            _warningsToSuppress = new HashSet<Warning>(warningsToSuppress);
            _classAccessor = classAccessor;
        }

        public void Check()
        {
            if (_warningsToSuppress.Contains(Warning.NULL_FIELDS))
                return;

            // FieldInspector<T> inspector = new FieldInspector<T>(classAccessor);
            // inspector.check(new NullPointerExceptionFieldCheck());
        }
    }
}

