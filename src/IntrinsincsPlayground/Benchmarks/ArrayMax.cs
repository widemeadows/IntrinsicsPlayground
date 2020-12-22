using System.Linq;
using BenchmarkDotNet.Attributes;
using IntrinsicsPlayground.Intrinsics.ArrayIntrinsics;
using JM.LinqFaster.SIMD;

namespace IntrinsicsPlayground.Benchmarks
{
    public class ArrayMax : ArrayBenchmarkBase
    {
        [Benchmark]
        public int Max_LINQ()
        {
            return ArrayOfInts.Max();
        }

        [Benchmark]
        public int Max_Simple()
        {
            return Max_Simple(ArrayOfInts);
        }

        [Benchmark]
        public int Max_LinqFasterLib()
        {
            return ArrayOfInts.MaxS();
        }

        [Benchmark(Baseline = true)]
        public int Max_Avx()
        {
            return ArrayIntrinsics.Max_Avx2(ArrayOfInts);
        }

        public static int Max_Simple(int[] array)
        {
            var max = int.MinValue;
            for (var i = 0; i < array.Length; i++)
            {
                var item = array[i];
                if (item > max)
                    max = item;
            }
            return max;
        }
    }
}
