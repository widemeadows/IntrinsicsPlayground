using FluentAssertions;
using IntrinsicsPlayground.Benchmarks.Intrinsics;
using Xunit;

namespace IntrinsicsPlayground.Tests
{
    public class LeadingZeroCountTests
    {
        [Fact]
        public void Lzcnt_Equals_Reference()
        {
            var lzcnt = new LeadingZeroCount();
            var expected = lzcnt.LeadingZeroCount_Soft();
            var actual = lzcnt.LeadingZeroCount_HW();

            actual.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }

        [Fact]
        public void Avx2_Equals_Reference()
        {
            var lzcnt = new LeadingZeroCount();
            var expected = lzcnt.LeadingZeroCount_Soft();
            var actual = lzcnt.LeadingZeroCount_Avx2();

            actual.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }
    }
}
