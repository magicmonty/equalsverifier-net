using EqualsVerifier.TestHelpers;
using NUnit.Framework;
using Shouldly;
using System;
using EqualsVerifier.Util;
using System.Linq;
using EqualsVerifier.Util.Exceptions;

namespace EqualsVerifier.Integration.ExtendedContract
{
    [TestFixture]
    public class ArrayTest : IntegrationTestBase
    {
        const string REGULAR_EQUALS = "Array: == or regular Equals() used for field";
        const string REGULAR_HASHCODE = "Array: regular GetHashCode() used for field";
        const string MULTIDIMENSIONAL_EQUALS = "Multidimensional array: == or regular Equals() used for field";
        const string MULTIDIMENSIONAL_HASHCODE = "Multidimensional array: regular GetHashCode() used for field";
        const string FIELD_NAME = "array";

        [Test]
        public void GivenAPrimitiveArray_WhenRegularEqualsIsUsedInsteadOfArraysEquals_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<PrimitiveArrayRegularEquals>().Verify(),
                REGULAR_EQUALS, 
                FIELD_NAME);
        }

        [Test]
        public void GivenAPrimitiveArray_WhenRegularHashCodeIsUsedInsteadOfArraysHashCode_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<PrimitiveArrayRegularHashCode>().Verify(),
                REGULAR_HASHCODE, 
                FIELD_NAME);
        }

        [Test]
        public void GivenAPrimitiveArray_WhenCorrectMethodsAreUsed_ThenSucceed()
        {
            EqualsVerifier
                .ForType<PrimitiveArrayCorrect>()
                .Verify();
        }

        [Test]
        public void GivenAMultidimensionalArray_WhenArraysEqualsIsUsedInsteadOfDeepEquals_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<MultidimensionalArrayArraysEquals>().Verify(),
                MULTIDIMENSIONAL_EQUALS, 
                FIELD_NAME);
        }

        [Test]
        public void GivenAMultidimensionalArray_WhenRegularHashCodeIsUsedInsteadOfDeepHashCode_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<MultidimensionalArrayRegularHashCode>().Verify(),
                MULTIDIMENSIONAL_HASHCODE, 
                FIELD_NAME);
        }

        [Test]
        public void GivenAMultidimensionalArray_WhenArraysHashCodeIsUsedInsteadOfDeepHashCode_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<MultidimensionalArrayArraysHashCode>().Verify(),
                MULTIDIMENSIONAL_HASHCODE, 
                FIELD_NAME);
        }

        [Test]
        public void GivenAMultidimensionalArray_WhenCorrectMethodsAreUsed_ThenSucceed()
        {
            EqualsVerifier
                .ForType<MultidimensionalArrayCorrect>()
                .Verify();
        }

        [Test]
        public void GivenTwoMultidimensionalArrays_WhenShallowHashCodeIsUsedOnSecondArray_ThenFailWithCorrectMessage()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<TwoMultidimensionalArraysShallowHashCodeForSecond>().Verify(),
                "second", 
                MULTIDIMENSIONAL_HASHCODE);
        }

        [Test]
        public void GivenAThreedimensionalArray_WhenCorrectMethodsAreUsed_ThenSucceed()
        {
            EqualsVerifier
                .ForType<ThreeDimensionalArrayCorrect>()
                .Verify();
        }

        [Test]
        public void WhenClassContainsARecursionButAlsoAMutltiDimensionalArray_ThenFailWithRecursionError()
        {
            ExpectFailureWithCause<ReflectionException>(
                () => EqualsVerifier.ForType<MultiDimensionalArrayAndRecursion.Board>().Verify()
            );
        }

        [Test]
        public void GivenAnObjectArray_WhenRegularEqualsIsUsedInsteadOfArraysEquals_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<ObjectArrayRegularEquals>().Verify(),
                REGULAR_EQUALS, 
                FIELD_NAME);
        }

        [Test]
        public void GivenAnObjectArray_WhenRegularHashCodeIsUsedInsteadOfArraysHashCode_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<ObjectArrayRegularHashCode>().Verify(),
                REGULAR_HASHCODE, 
                FIELD_NAME);
        }

        [Test]
        public void GivenAnObjectArray_WhenCorrectMethodsAreUsed_ThenSucceed()
        {
            EqualsVerifier
                .ForType<ObjectArrayCorrect>()
                .Verify();
        }

        [Test]
        public void GivenAnArrayAndAnUnusedField_WhenCorrectMethodsAreUsed_ThenSucceed()
        {
            EqualsVerifier
                .ForType<ArrayAndSomethingUnused>()
                .Verify();
        }

        [Test]
        public void GivenArrayFields_WhenArraysAreNotUsedInEquals_ThenSucceed()
        {
            EqualsVerifier
                .ForType<ArrayAndNoEquals>()
                .Verify();
        }

        #pragma warning disable 414
        internal sealed class PrimitiveArrayRegularEquals
        {
            readonly int[] _array;

            public PrimitiveArrayRegularEquals(int[] array)
            {
                _array = array;
            }

            public override bool Equals(object obj)
            {
                var other = obj as PrimitiveArrayRegularEquals;
                return other != null && _array.NullSafeEquals(other._array);
            }

            public override int GetHashCode()
            {
                return (_array == null) ? 0 : _array.GetHashCode();
            }
        }

        internal sealed class PrimitiveArrayRegularHashCode
        {
            readonly int[] _array;

            public PrimitiveArrayRegularHashCode(int[] array)
            {
                _array = array;
            }

            public override bool Equals(object obj)
            {
                var other = obj as PrimitiveArrayRegularHashCode;
                return other != null && _array.ArrayDeeplyEquals(other._array);
            }

            public override int GetHashCode()
            {
                return _array.GetNullSafeHashCode();
            }
        }

        internal sealed class PrimitiveArrayCorrect
        {
            readonly int[] _array;

            public PrimitiveArrayCorrect(int[] array)
            {
                _array = array;
            }

            public override bool Equals(object obj)
            {
                var other = obj as PrimitiveArrayCorrect;
                return other != null && _array.ArrayDeeplyEquals(other._array);
            }

            public override int GetHashCode()
            {
                return (_array == null) ? 0 : _array.GetDeepHashCode();
            }
        }

        internal sealed class MultidimensionalArrayArraysEquals
        {
            readonly int[][] _array;

            public MultidimensionalArrayArraysEquals(int[][] array)
            {
                _array = array;
            }

            public override bool Equals(object obj)
            {
                var other = obj as MultidimensionalArrayArraysEquals;
                return other != null && object.Equals(_array, other._array);
            }

            public override int GetHashCode()
            {
                return (_array == null) ? 0 : _array.GetDeepHashCode();
            }
        }

        internal sealed class MultidimensionalArrayRegularHashCode
        {
            readonly int[][] _array;

            public MultidimensionalArrayRegularHashCode(int[][] array)
            {
                _array = array;
            }

            public override bool Equals(object obj)
            {
                var other = obj as MultidimensionalArrayRegularHashCode;
                return other != null && _array.ArrayDeeplyEquals(other._array);
            }

            public override int GetHashCode()
            {
                return (_array == null) ? 0 : _array.GetHashCode();
            }
        }

        internal sealed class MultidimensionalArrayArraysHashCode
        {
            readonly int[][] _array;

            public MultidimensionalArrayArraysHashCode(int[][] array)
            {
                _array = array;
            }

            public override bool Equals(object obj)
            {
                var other = obj as MultidimensionalArrayArraysHashCode;
                return other != null && _array.ArrayDeeplyEquals(other._array);
            }

            public override int GetHashCode()
            {
                return _array.GetNullSafeHashCode();
            }
        }

        internal sealed class MultidimensionalArrayCorrect
        {
            readonly int[][] _array;

            public MultidimensionalArrayCorrect(int[][] array)
            {
                _array = array;
            }

            public override bool Equals(object obj)
            {
                var other = obj as MultidimensionalArrayCorrect;
                return other != null && _array.ArrayDeeplyEquals(other._array);
            }

            public override int GetHashCode()
            {
                return (_array == null) ? 0 : _array.GetDeepHashCode();
            }
        }

        internal sealed class TwoMultidimensionalArraysShallowHashCodeForSecond
        {
            readonly object[][] _first;
            readonly object[][] _second;

            public TwoMultidimensionalArraysShallowHashCodeForSecond(object[][] first, object[][] second)
            {
                _first = first;
                _second = second;
            }

            public override bool Equals(object obj)
            {
                var other = obj as TwoMultidimensionalArraysShallowHashCodeForSecond;
                return other != null
                && _first.ArrayDeeplyEquals(other._first)
                && _second.ArrayDeeplyEquals(other._second);
            }

            public override int GetHashCode()
            {
                const int prime = 31;
                var result = 1;
                result = (result * prime) + _first.GetDeepHashCode();
                result = (result * prime) + _second.GetNullSafeHashCode();
                return result;
            }
        }

        internal sealed class ThreeDimensionalArrayCorrect
        {
            readonly int[][][] _array;

            public ThreeDimensionalArrayCorrect(int[][][] array)
            {
                _array = array;
            }

            public override bool Equals(object obj)
            {
                var other = obj as ThreeDimensionalArrayCorrect;
                return other != null && _array.ArrayDeeplyEquals(other._array);
            }

            public override int GetHashCode()
            {
                return (_array == null) ? 0 : _array.GetDeepHashCode();
            }
        }

        internal static class MultiDimensionalArrayAndRecursion
        {
            internal sealed class Board
            {
                readonly object[][] _grid;
                BoardManipulator _manipulator;

                public Board(Object[][] grid)
                {
                    _grid = grid;
                    _manipulator = new BoardManipulator(this);
                }
            }

            internal sealed class BoardManipulator
            {
                readonly Board _board;

                public BoardManipulator(Board board)
                {
                    _board = board;
                }
            }
        }

        internal sealed class ObjectArrayRegularEquals
        {
            readonly object[] _array;

            public ObjectArrayRegularEquals(Object[] array)
            {
                _array = array;
            }

            public override bool Equals(object obj)
            {
                var other = obj as ObjectArrayRegularEquals;
                return other != null && _array.NullSafeEquals(other._array);
            }

            public override int GetHashCode()
            {
                return (_array == null) ? 0 : _array.GetHashCode();
            }
        }

        internal sealed class ObjectArrayRegularHashCode
        {
            readonly object[] _array;

            public ObjectArrayRegularHashCode(Object[] array)
            {
                _array = array;
            }

            public override bool Equals(object obj)
            {
                var other = obj as ObjectArrayRegularHashCode;
                return other != null && _array.ArrayDeeplyEquals(other._array);
            }

            public override int GetHashCode()
            {
                return _array.GetNullSafeHashCode();
            }
        }

        internal sealed class ObjectArrayCorrect
        {
            readonly object[] _array;

            public ObjectArrayCorrect(Object[] array)
            {
                _array = array;
            }

            public override bool Equals(object obj)
            {
                var other = obj as ObjectArrayCorrect;
                return other != null && _array.ArrayDeeplyEquals(other._array);
            }

            public override int GetHashCode()
            {
                return (_array == null) ? 0 : _array.GetDeepHashCode();
            }
        }

        internal sealed class ArrayAndSomethingUnused
        {
            readonly int[] _array;
            readonly int _i;

            public ArrayAndSomethingUnused(int[] array, int i)
            {
                _array = array;
                _i = i;
            }

            public override bool Equals(object obj)
            {
                var other = obj as ArrayAndSomethingUnused;
                return other != null && _array.ArrayDeeplyEquals(other._array);
            }

            public override int GetHashCode()
            {
                return _array.GetDeepHashCode();
            }
        }

        internal sealed class ArrayAndNoEquals
        {
            readonly int[] _ints;
            readonly object[] _objects;
            readonly int[][] _arrays;

            public ArrayAndNoEquals(int[] ints, Object[] objects, int[][] arrays)
            {
                _ints = ints;
                _objects = objects;
                _arrays = arrays;
            }
        }
        #pragma warning restore 414
    }
}

