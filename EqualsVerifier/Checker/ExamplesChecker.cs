using System.Collections.Generic;
using System;

namespace EqualsVerifier.Checker
{
    public class ExamplesChecker<T> : IChecker
    {
        readonly Type _type;
        readonly IEnumerable<T> _equalExamples;
        readonly IEnumerable<T> _unequalExamples;

        public ExamplesChecker(IEnumerable<T> equalExamples, IEnumerable<T> unequalExamples)
        {
            _type = typeof(T);
            _unequalExamples = unequalExamples;
            _equalExamples = equalExamples;
        }

        public void Check()
        {
        }
    }
}

