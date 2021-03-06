﻿using System;
using System.Linq;
using IntrinsicsPlayground.Benchmarks.Arrays;
using IntrinsicsPlayground.Intrinsics.ArrayIntrinsics;
using Xunit;

namespace IntrinsicsPlayground.Tests
{
    public class ArrayIntrinsicsTests
    {
        [Fact]
        public void ArrayIntrinsics_SequenceEqual_Avx()
        {
            for (var i = 0; i < 1024; i++)
            {
                var arrayOfFloats1 = Enumerable.Range(0, i).Select(n => n / 2.0f).ToArray();
                var arrayOfFloats2 = arrayOfFloats1.ToArray();

                var expected = Enumerable.SequenceEqual(arrayOfFloats1, arrayOfFloats2);
                var actual = ArrayIntrinsics.SequenceEqual_Avx(arrayOfFloats1, arrayOfFloats2);
                Assert.Equal(expected, actual);

                // so now the arrays are not equal
                if (arrayOfFloats2.Length > 42)
                {
                    arrayOfFloats2[42] = -1.0f;
                    actual = ArrayIntrinsics.SequenceEqual_Avx(arrayOfFloats1, arrayOfFloats2);
                    Assert.False(actual);
                }
            }
        }

        [Fact]
        public void ArrayIntrinsics_Reverse_Sse2()
        {
            for (var i = 0; i < 1024; i++)
            {
                var arrayOfInts1 = Enumerable.Range(0, i).ToArray();
                var arrayOfInts2 = arrayOfInts1.ToArray();

                Array.Reverse(arrayOfInts1);
                ArrayIntrinsics.Reverse_Sse2(arrayOfInts2);

                Assert.True(arrayOfInts1.SequenceEqual(arrayOfInts2));
            }
        }

        [Fact]
        public void ArrayIntrinsics_IsSorted_Avx2()
        {
            for (var i = 0; i < 1024; i++)
            {
                var sortedArray = Enumerable.Range(0, i).ToArray();
                Assert.True(ArrayIntrinsics.IsSorted_Avx2(sortedArray));

                if (i > 2)
                {
                    var unsortedArray = sortedArray.Concat(sortedArray).ToArray();
                    Assert.False(ArrayIntrinsics.IsSorted_Avx2(unsortedArray));
                }
            }
        }

        [Fact]
        public void ArrayIntrinsics_IsSorted_Simple()
        {
            for (var i = 0; i < 1024; i++)
            {
                var sortedArray = Enumerable.Range(0, i).ToArray();
                Assert.True(ArrayIsSorted.IsSorted_Simple(sortedArray));

                if (i > 2)
                {
                    var unsortedArray = sortedArray.Concat(sortedArray).ToArray();
                    Assert.False(ArrayIsSorted.IsSorted_Simple(unsortedArray));
                }
            }
        }

        [Fact]
        public void ArrayIntrinsics_IsSorted_Simple2()
        {
            for (var i = 0; i < 1024; i++)
            {
                var sortedArray = Enumerable.Range(0, i).ToArray();
                Assert.True(ArrayIsSorted.IsSorted_Simple2(sortedArray));

                if (i > 2)
                {
                    var unsortedArray = sortedArray.Concat(sortedArray).ToArray();
                    Assert.False(ArrayIsSorted.IsSorted_Simple2(unsortedArray));
                }
            }
        }

        [Fact]
        public void ArrayIntrinsics_IndexOf_Avx2()
        {
            for (var i = 1; i < 1024; i++)
            {
                var array = Enumerable.Range(0, i).ToArray();
                var item = array[array.Length / 2];
                var expectedIndex = Array.IndexOf(array, item);
                var actualIndex = ArrayIntrinsics.IndexOf_Avx2(array, item);
                Assert.Equal(expectedIndex, actualIndex);
            }
        }

        [Fact]
        public void ArrayIntrinsics_IndexOf_Sse41()
        {
            for (var i = 1; i < 1024; i++)
            {
                var array = Enumerable.Range(0, i).ToArray();
                var item = array[array.Length / 2];
                var expectedIndex = Array.IndexOf(array, item);
                var actualIndex = ArrayIntrinsics.IndexOf_Sse41(array, item);
                Assert.Equal(expectedIndex, actualIndex);
            }
        }

        [Fact(Skip = "The array is required to be aligned, which currently isn't guaranteed")]
        public void ArrayIntrinsics_IndexOf_Sse41Aligned()
        {
            for (var i = 1; i < 1024; i++)
            {
                var array = Enumerable.Range(0, i).ToArray();
                var item = array[array.Length / 2];
                var expectedIndex = Array.IndexOf(array, item);
                var actualIndex = ArrayIntrinsics.IndexOf_Sse41_aligned(array, item);
                Assert.Equal(expectedIndex, actualIndex);
            }
        }

        [Fact]
        public void ArrayIntrinsics_Contains_Avx2()
        {
            for (var i = 2; i < 1024; i++)
            {
                var array = Enumerable.Range(0, i).ToArray();
                var item = array[array.Length / 2];

                Assert.True(ArrayIntrinsics.Contains_Avx2(array, item));
                Assert.False(ArrayIntrinsics.Contains_Avx2(array, -42));
            }
        }

        [Fact]
        public void ArrayIntrinsics_Max()
        {
            for (var i = 2; i < 1024; i++)
            {
                var array = Enumerable.Range(0, i).Concat(Enumerable.Range(0, i).Reverse()).ToArray(); // 0 1 2 3 2 1 0 (for i==4)

                var expected = array.Max();
                var actual = ArrayIntrinsics.Max_Avx2(array);

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void ArrayIntrinsics_Max_float()
        {
            for (var i = 2; i < 1024; i++)
            {
                var array = Enumerable.Range(0, i).Concat(Enumerable.Range(0, i).Reverse()).Select(t => (float)t).ToArray(); // 0 1 2 3 2 1 0 (for i==4)

                var expected = array.Max();
                var actual = ArrayIntrinsics.Max_Avx(array);

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void ArrayIntrinsics_Sum()
        {
            for (var i = 0; i < 1024; i++)
            {
                var array = Enumerable.Range(0, i).Select(n => n / 2.0f).ToArray();

                var expected = array.Sum();
                var actual = ArrayIntrinsics.Sum_Avx(array);

                Assert.Equal(expected, actual);
            }
        }
    }
}
