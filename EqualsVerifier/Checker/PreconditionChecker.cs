using System.Collections;
using System.Collections.Generic;
using System;
using EqualsVerifier.Util;
using System.Linq;


namespace EqualsVerifier.Checker
{
    public class PreconditionChecker<T> : AbstractChecker where T:class
    {
        readonly Type _type;
        readonly IEnumerable<T> _equalExamples;
        readonly IEnumerable<T> _unequalExamples;

        public PreconditionChecker(IEnumerable<T> equalExamples, IEnumerable<T> unequalExamples)
        {
            _unequalExamples = unequalExamples;
            _equalExamples = equalExamples;
            _type = _equalExamples.Any() ? _equalExamples.First().GetType() : typeof(T);
        }

        public override void Check()
        {
            AssertTrue(
                ObjectFormatter.Of("Precondition: no examples."), 
                _unequalExamples.Any());

            foreach (var example in _equalExamples)
            {
                AssertTrue(
                    ObjectFormatter.Of(
                        "Precondition:\n  %%\nand\n  %%\nare of different classes",
                        _equalExamples.First(),
                        example),
                    _type.IsInstanceOfType(example));
            }

            foreach (var example in _unequalExamples)
            {
                AssertTrue(
                    ObjectFormatter.Of(
                        "Precondition:\n  %%\nand\n  %%\nare of different classes",
                        _unequalExamples.First(),
                        example),
                    _type.IsInstanceOfType(example));
            }        
        }
    }
}

