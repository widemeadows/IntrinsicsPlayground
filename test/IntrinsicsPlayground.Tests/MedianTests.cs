using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace IntrinsicsPlayground.Tests
{
    public class MedianTests
    {
        private readonly int[] _unsortedArray;
        public MedianTests()
        {
            var rand = new Random();
            _unsortedArray = Enumerable
                .Range(0, 1024)
                .OrderBy(i => rand.Next())
                .ToArray();
        }

        [Fact]
        public void Median5_Soft()
        {
            var array = _unsortedArray.Take(5).ToArray();
            var expected = _unsortedArray.Take(5).OrderBy(v => v).Skip(5 / 2).First();

            var median = Misc.Median.Medians.Median5_Soft(array[0], array[1], array[2], array[3], array[4]);
            median.Should().Be(expected);
        }

        [Fact]
        public void Median5_Soft2()
        {
            var array = _unsortedArray.Take(5).ToArray();
            var expected = _unsortedArray.Take(5).OrderBy(v => v).Skip(5 / 2).First();

            var median = Misc.Median.Medians.Median5_Soft2(array[0], array[1], array[2], array[3], array[4]);
            median.Should().Be(expected);
        }

        [Fact]
        public void Median5_Relaxed2()
        {
            var array = _unsortedArray.Take(5).ToArray();
            var expected = _unsortedArray.Take(5).OrderBy(v => v).Skip(5 / 2).First();

            var median = Misc.Median.Medians.Median5_Relaxed2(array[0], array[1], array[2], array[3], array[4]);
            median.Should().Be(expected);
        }
    }
}
