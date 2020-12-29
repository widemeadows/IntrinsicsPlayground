using System;
using System.Linq;
using System.Runtime.CompilerServices;
using FluentAssertions;
using Xunit;

namespace IntrinsicsPlayground.Tests
{
    public class SortTests
    {
        private readonly int[] _unsortedArray;
        private readonly int[] _sortedArray;

        public SortTests()
        {
            var rand = new Random();
            _unsortedArray = Enumerable
                .Range(0, 1024)
                .OrderBy(i => rand.Next())
                .ToArray();

            _sortedArray = _unsortedArray.ToArray();
            Array.Sort(_sortedArray);
        }

        [Fact]
        public void BubbleSort()
        {
            var array = _unsortedArray.ToArray();
            Misc.Sorting.BubbleSort.Sort(array);
            array.Should().BeEquivalentTo(_sortedArray, options => options.WithStrictOrdering());
        }

        [Fact]
        public void InsertionSort()
        {
            var array = _unsortedArray.ToArray();
            Misc.Sorting.InsertionSort.Sort(array);
            array.Should().BeEquivalentTo(_sortedArray, options => options.WithStrictOrdering());
        }

        [Fact]
        public void MergeSort()
        {
            var array = _unsortedArray.ToArray();
            Misc.Sorting.MergeSort.Sort(array);
            array.Should().BeEquivalentTo(_sortedArray, options => options.WithStrictOrdering());
        }

        [Fact]
        public void RadixSort()
        {
            var array = _unsortedArray.ToArray();
            Misc.Sorting.RadixSort.Sort(array);
            array.Should().BeEquivalentTo(_sortedArray, options => options.WithStrictOrdering());
        }

        [Fact]
        public void ClassicQuicksort()
        {
            var array = _unsortedArray.ToArray();
            Misc.Sorting.ClassicQuicksort.Sort(array);
            array.Should().BeEquivalentTo(_sortedArray, options => options.WithStrictOrdering());
        }

        [Fact]
        public void ThreeWayQuicksort()
        {
            var array = _unsortedArray.ToArray();
            Misc.Sorting.Quicksort3Way.Sort(array);
            array.Should().BeEquivalentTo(_sortedArray, options => options.WithStrictOrdering());
        }

        [Fact]
        public void JavaSort()
        {
            var array = _unsortedArray.ToArray();
            Misc.Sorting.JavaSort.Sort(array);
            array.Should().BeEquivalentTo(_sortedArray, options => options.WithStrictOrdering());
        }

        [Fact]
        public void HeapSort()
        {
            var array = _unsortedArray.ToArray();
            Misc.Sorting.HeapSort.Sort(array);
            array.Should().BeEquivalentTo(_sortedArray, options => options.WithStrictOrdering());
        }

        [Fact]
        public void SortFloatsAsIntegers()
        {
            // This test implements a stunt that might be pulled for sorting floating-point values
            // using integer-specific sorting algorithms, given that some oddities are acceptable.

            // When dealing with 32-bit floats of IEEE 754, the bit representation of a float is
            //
            //      S EEEE EEEE MMM MMMM MMMM MMMM MMMM MMMM
            //
            // with
            //
            //      S = sign
            //      E = exponent
            //      M = mantissa
            //
            // where the float representation is (_basically_)
            //
            //      x = s * m * 2^e
            //
            // Because of this layout, the most significant bits are in the leftmost position (if we
            // ignore the sign bit), and the least significant bit is to the right.
            //
            // If we treat a float as a signed integer, then
            // - positive floats should sort like regular positive integers, but
            // - absolutely small negative floats (e.g. -1) will be interpreted as absolutely large
            //   negative integers (e.g. -1082130432), while
            // - absolutely large negative floats (e.g. Single.MinValue) will be treated as absolutely large
            //   negative integers (e.g. -8388609).
            //
            // As a result, a sorting on re-interpreted floats should show
            // - negative floats in descending order (-1, -2, -3, ...) followed by
            // - non-negative floats in ascending order (0, 1, 2, 3, ...).

            // We generate a sequence of floats, ranging from -512 .. 0 ... 511
            var floats = _sortedArray.Select(x => (float)x - 512f).ToArray();

            // We now cast the floats to integers and sort the array.
            Array.Sort(Unsafe.As<int[]>(floats));

            // Negative values first, descending order.
            for (var i = 0; i < 511; ++i)
            {
                floats[i].Should()
                    .BeNegative("because we expect negative values first")
                    .And.BeGreaterThan(floats[i + 1], "because we expect negative values to be sorted in descending order");
            }

            // Zero is crammed between the two segments.
            floats[512].Should().Be(0, "because we expect zero to be in between negative and positive values");

            // Positive values in ascending order (1, 2, 3, ...).
            for (var i = 513; i < floats.Length - 1; ++i)
            {
                floats[i].Should()
                    .BePositive("because we expect positive values last")
                    .And.BeLessThan(floats[i + 1], "because we expect positive values to be sorted in ascending order");
            }
        }
    }
}
