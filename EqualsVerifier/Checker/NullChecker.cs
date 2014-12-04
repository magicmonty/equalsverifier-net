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
            _warningsToSuppress = warningsToSuppress;
            _classAccessor = classAccessor;
        }

        public void Check()
        {

        }
    }
}

