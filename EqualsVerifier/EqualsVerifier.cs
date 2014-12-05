using System;
using EqualsVerifier.Util.Exceptions;
using EqualsVerifier.Util;
using System.Collections.Generic;
using System.Linq;
using EqualsVerifier.Checker;

namespace EqualsVerifier
{
    public static class EqualsVerifier
    {
        public static EqualsVerifier<T> ForType<T>() where T: class
        {
            var equalExamples = Enumerable.Empty<T>();
            var unequalExamples = Enumerable.Empty<T>();

            return new EqualsVerifier<T>(equalExamples, unequalExamples);
        }

        public static EqualsVerifier<T> ForExamples<T>(T first, T second, params T[] more) where T: class
        {
            var equalExamples = Enumerable.Empty<T>();
            var unequalExamples = BuildListOfAtLeastTwo(first, second, more);

            return new EqualsVerifier<T>(equalExamples, unequalExamples);
        }

        public static RelaxedEqualsVerifierHelper<T> ForRelaxedEqualExamples<T>(T first, T second, params T[] more) where T: class
        {
            return new RelaxedEqualsVerifierHelper<T>(BuildListOfAtLeastTwo(first, second, more));
        }

        static IEnumerable<T> BuildListOfAtLeastTwo<T>(T first, T second, params T[] more) where T: class
        {
            if (!first.GetType().IsValueType && first == null)
            {
                throw new ArgumentNullException("first");
            }

            if (!second.GetType().IsValueType && second == null)
            {
                throw new ArgumentNullException("second");
            }

            var result = new List<T>();

            result.Add(first);
            result.Add(second);
            AddArrayElementsToList(result, more);

            return result;
        }

        static void AddArrayElementsToList<T>(List<T> list, params T[] more) where T: class
        {
            if (more == null)
                return;

            if (more.Any(e => e == null))
                throw new ArgumentException("One of the examples is null.");

            list.AddRange(more);
        }

        static IEnumerable<T> BuildListOfAtLeastOne<T>(T first, params T[] more) where T: class
        {
            if (first == null)
                throw new ArgumentNullException("first");

            var result = new List<T>();

            result.Add(first);
            AddArrayElementsToList(result, more);

            return result;
        }

        public class RelaxedEqualsVerifierHelper<T> where T: class
        {
            readonly IEnumerable<T> _equalExamples;

            internal RelaxedEqualsVerifierHelper(IEnumerable<T> examples)
            {
                _equalExamples = examples;
            }

            public EqualsVerifier<T> AndUnequalExample(T example)
            {
                var unequalExamples = EqualsVerifier.BuildListOfAtLeastOne(example);
                return new EqualsVerifier<T>(_equalExamples, unequalExamples);
            }

            public EqualsVerifier<T> AndUnequalExamples(T first, params T[] more)
            {
                var unequalExamples = BuildListOfAtLeastOne(first, more);
                return new EqualsVerifier<T>(_equalExamples, unequalExamples);
            }
        }
    }

    public class EqualsVerifier<T> where T: class
    {
        readonly Type _type;
        readonly List<T> _equalExamples;
        readonly List<T> _unequalExamples;
        readonly StaticFieldValueStash _stash;
        readonly PrefabValues _prefabValues;
        readonly HashSet<Warning> _warningsToSuppress = new HashSet<Warning>();

        bool _usingGetClass;
        bool _allFieldsShouldBeUsed;
        ISet<string> _allFieldsShouldBeUsedExceptions = new HashSet<string>();
        bool _hasRedefinedSubclass;
        Type _redefinedSubclass;

        internal EqualsVerifier(IEnumerable<T> equalExamples, IEnumerable<T> unequalExamples)
        {
            _type = typeof(T);
            _equalExamples = (equalExamples ?? Enumerable.Empty<T>()).ToList();
            _unequalExamples = (unequalExamples ?? Enumerable.Empty<T>()).ToList();

            _stash = new StaticFieldValueStash();
            _prefabValues = new PrefabValues(_stash);
            NetApiPrefabValues.AddTo(_prefabValues);
        }

        public EqualsVerifier<T> Suppress(params Warning[] warnings)
        {
            foreach (var warning in warnings)
                _warningsToSuppress.Add(warning);

            return this;
        }

        public EqualsVerifier<T> WithPrefabValues<TOther>(TOther red, TOther black) where TOther: class
        {
            var otherType = typeof(TOther);

            if (!otherType.IsValueType && (red == null || black == null))
            {
                throw new NullReferenceException("One or both values are null.");
            }

            if (red.Equals(black))
            {
                throw new ArgumentException("Both values are equal.");
            }

            _prefabValues.Put(otherType, red, black);

            return this;
        }

        public EqualsVerifier<T> UsingGetClass()
        {
            _usingGetClass = true;
            return this;
        }

        public EqualsVerifier<T> AllFieldsShouldBeUsed()
        {
            _allFieldsShouldBeUsed = true;
            return this;
        }

        public EqualsVerifier<T> AllFieldsShouldBeUsedExcept(params string[] fields)
        {
            _allFieldsShouldBeUsed = true;
            _allFieldsShouldBeUsedExceptions = new HashSet<string>(fields);

            var actualFieldNames = new HashSet<string>(FieldEnumerable.Of(_type).Select(f => f.Name));

            foreach (var field in _allFieldsShouldBeUsedExceptions)
            {
                if (!actualFieldNames.Contains(field))
                    throw new ArgumentException("Class " + _type.Name + " does not contain field " + field + ".");
            }

            return this;
        }

        public EqualsVerifier<T> WithRedefinedSuperclass()
        {
            _hasRedefinedSubclass = true;
            return this;
        }

        public EqualsVerifier<T> WithRedefinedSubclass<TSubClass>() where TSubClass:T
        {
            _redefinedSubclass = typeof(TSubClass);
            return this;
        }

        public void Verify()
        {
            try
            {
                _stash.Backup(_type);
                PerformVerification();
            }
            catch (InternalException e)
            {
                HandleError(e, e.InnerException);
            }
            catch (Exception e)
            {
                HandleError(e, e);
            }
            finally
            {
                _stash.RestoreAll();
            }
        }

        static void HandleError(Exception messageContainer, Exception trueCause)
        {
            var showCauseExceptionInMessage = trueCause != null && trueCause.Equals(messageContainer);

            var message = ObjectFormatter.Of(
                              "%%%%",
                              showCauseExceptionInMessage ? trueCause.GetType().Name + ": " : string.Empty,
                              messageContainer.Message ?? string.Empty);

            throw new AssertionException(message, trueCause);
        }

        void PerformVerification()
        {
            if (_type.IsEnum)
                return;
                
            var classAccessor = ClassAccessor.Of(_type, _prefabValues, _warningsToSuppress.Contains(Warning.ATTRIBUTE));
            VerifyWithoutExamples(classAccessor);
            EnsureUnequalExamples(classAccessor);
            VerifyWithExamples(classAccessor);
        }

        void VerifyWithoutExamples(ClassAccessor classAccessor)
        {
            RunCheckers(new IChecker[]
            {
                new SignatureChecker<T>(),
                new AbstractDelegationChecker<T>(classAccessor),
                new NullChecker<T>(classAccessor, _warningsToSuppress)
            });
        }

        void EnsureUnequalExamples(ClassAccessor classAccessor)
        {
            if (_unequalExamples.Count > 0)
                return;

            _unequalExamples.Add((T)classAccessor.GetRedObject());
            _unequalExamples.Add((T)classAccessor.GetBlackObject());
        }

        void VerifyWithExamples(ClassAccessor classAccessor)
        {
            RunCheckers(new IChecker[]
            {
                new PreconditionChecker<T>(_equalExamples, _unequalExamples),
                new ExamplesChecker<T>(_equalExamples, _unequalExamples),
                new HierarchyChecker<T>(
                    classAccessor,
                    _warningsToSuppress,
                    _usingGetClass,
                    _hasRedefinedSubclass,
                    _redefinedSubclass),
                new FieldsChecker<T>(
                    classAccessor,
                    _warningsToSuppress,
                    _allFieldsShouldBeUsed,
                    _allFieldsShouldBeUsedExceptions)
            });
        }

        static void RunCheckers(IEnumerable<IChecker> checkers)
        {
            checkers.ToList().ForEach(c => c.Check());
        }
    }
}

