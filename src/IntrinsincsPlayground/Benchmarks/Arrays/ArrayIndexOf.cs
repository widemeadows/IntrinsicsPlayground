using BenchmarkDotNet.Attributes;
using IntrinsicsPlayground.Intrinsics.ArrayIntrinsics;

namespace IntrinsicsPlayground.Benchmarks.Arrays
{
    public class ArrayIndexOf : ArrayBenchmarkBase
    {
        //[Benchmark]
        //public int IndexOf_BCL()
        //{
        //    return Array.IndexOf(ArrayOfInts, ArrayOfInts[ArrayOfInts.Length / 2]);
        //}

        //[Benchmark]
        //public int IndexOf_Avx2()
        //{
        //    return ArrayIntrinsics.IndexOf_Avx2(ArrayOfInts, ArrayOfInts[ArrayOfInts.Length / 2]);
        //}

        [Benchmark(Baseline = true)]
        public int IndexOf_Sse41() => ArrayIntrinsics.IndexOf_Sse41(ArrayOfInts, ArrayOfInts[ArrayOfInts.Length / 2]);

        [Benchmark]
        public int IndexOf_Sse41_aligned() => ArrayIntrinsics.IndexOf_Sse41_aligned(ArrayOfInts, ArrayOfInts[ArrayOfInts.Length / 2]);
    }
}
