using System.Collections.Generic;
using System;
using System.Linq;
using EqualsVerifier.Util;

namespace EqualsVerifier.Checker
{
    public class ExamplesChecker<T> : AbstractChecker where T: class
    {
        readonly Type _type;
        readonly T[] _equalExamples;
        readonly T[] _unequalExamples;

        public ExamplesChecker(IEnumerable<T> equalExamples, IEnumerable<T> unequalExamples)
        {
            _type = typeof(T);
            _unequalExamples = unequalExamples.ToArray();
            _equalExamples = equalExamples.ToArray();
        }

        public override void Check()
        {

            for (int i = 0; i < _equalExamples.Length; i++) {
                var reference = _equalExamples[i];
                CheckSingle(reference);

                for (int j = i + 1; j < _equalExamples.Length; j++) {
                    var other = _equalExamples[j];
                    CheckEqualButNotIdentical(reference, other);
                    CheckHashCode(reference, other);
                }

                foreach (var other in _unequalExamples) {
                    CheckDouble(reference, other);
                }
            }

            for (int i = 0; i < _unequalExamples.Length; i++) {
                var reference = _unequalExamples[i];
                CheckSingle(reference);

                for (int j = i + 1; j < _unequalExamples.Length; j++) {
                    var other = _unequalExamples[j];
                    CheckDouble(reference, other);
                }
            }
        }

        static void CheckEqualButNotIdentical(T reference, T other)
        {
            AssertFalse(
                ObjectFormatter.Of("Precondition: the same object appears twice:\n  %%", reference),
                reference == other);

            AssertFalse(
                ObjectFormatter.Of("Precondition: two identical objects appear:\n  %%", reference),
                IsIdentical(reference, other));

            AssertTrue(
                ObjectFormatter.Of("Precondition: not all equal objects are equal:\n  %%\nand\n  %%", reference, other),
                reference.Equals(other));
        }

        void CheckSingle(T reference)
        {
            var copy = (T)ObjectAccessor.Of(reference, _type).Copy();

            CheckReflexivity(reference);
            CheckNonNullity(reference);
            CheckTypeCheck(reference);
            CheckHashCode(reference, copy);
        }

        static void CheckDouble(T reference, T other)
        {
            CheckNotEqual(reference, other);
        }

        static void CheckNotEqual(T reference, T other)
        {
            AssertFalse(ObjectFormatter.Of("Precondition: the same object appears twice:\n  %%", reference),
                reference == other);
            AssertFalse(ObjectFormatter.Of("Precondition: two objects are equal to each other:\n  %%", reference),
                reference.Equals(other));
        }

        static void CheckReflexivity(T reference)
        {
            AssertEquals(
                ObjectFormatter.Of("Reflexivity: object does not equal itself:\n  %%", reference),
                reference, reference);
        }

        static void CheckNonNullity(T reference)
        {
            try {
                var nullity = reference.Equals(null);
                AssertFalse(
                    ObjectFormatter.Of("Non-nullity: true returned for null value"), 
                    nullity);
            }
            catch (NullReferenceException e) {
                Fail(ObjectFormatter.Of("Non-nullity: NullReferenceException thrown"), e);
            }
        }

        class SomethingElse
        {

        }

        static void CheckTypeCheck(T reference)
        {
            var somethingElse = new SomethingElse();
            try {
                reference.Equals(somethingElse);
            }
            catch (InvalidCastException) {
                Fail(ObjectFormatter.Of("Type-check: equals throws InvalidCastException.\nAdd an 'is' or GetType() check."));
            }
            catch (Exception e) {
                Fail(ObjectFormatter.Of("Type-check: equals throws %%.\nAdd an 'is' or GetType() check.", e.GetType().Name), e);
            }
        }

        static void CheckHashCode(T reference, T copy)
        {
            if (!reference.Equals(copy))
                return;

            ObjectFormatter f = ObjectFormatter.Of("GetHashCode: HashCodes should be equal:\n  %% (%%)\nand\n  %% (%%)", reference, reference.GetHashCode(), copy, copy.GetHashCode());
            AssertEquals(f, reference.GetHashCode(), copy.GetHashCode());
        }

        static bool IsIdentical(T reference, T other)
        {
            foreach (var field in reference.GetType().GetFields(FieldHelper.AllFields)) {
                try {
                    if (!NullSafeEquals(field.GetValue(reference), field.GetValue(other)))
                        return false;
                }
                catch (Exception) {
                    return false;
                }
            }

            return true;
        }

        static bool NullSafeEquals(object x, object y)
        {
            return x == null ? y == null : x.Equals(y);
        }
    }
}

