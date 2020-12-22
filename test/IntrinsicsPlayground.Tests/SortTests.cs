using System;
using System.Linq;
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
        public void DualPivotQuicksort()
        {
            var array = _unsortedArray.ToArray();
            Misc.Sorting.DualPivotQuicksort.Sort(array);
            array.Should().BeEquivalentTo(_sortedArray, options => options.WithStrictOrdering());
        }

        [Fact]
        public void HeapSort()
        {
            var array = _unsortedArray.ToArray();
            Misc.Sorting.HeapSort.Sort(array);
            array.Should().BeEquivalentTo(_sortedArray, options => options.WithStrictOrdering());
        }
    }
}
