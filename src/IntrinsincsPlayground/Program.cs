using System;
using System.Runtime.Intrinsics.X86;
using BenchmarkDotNet.Running;
using IntrinsicsPlayground.Benchmarks;

namespace IntrinsicsPlayground
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            if (!Sse41.IsSupported || !Avx2.IsSupported)
                throw new NotSupportedException(":(");

            BenchmarkRunner.Run<ArrayIndexOf>();

            // Sorting:
            //BenchmarkRunner.Run<SortingAlreadySortedArray>();
            //BenchmarkRunner.Run<SortingAlreadySortedButReversedArray>();
            //BenchmarkRunner.Run<SortingRandomArray>();
            //BenchmarkRunner.Run<SortingNearlySortedArray>();
        }
    }
}
