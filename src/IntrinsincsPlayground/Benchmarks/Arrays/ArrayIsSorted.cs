using System.Linq;
using BenchmarkDotNet.Attributes;
using IntrinsicsPlayground.Intrinsics.ArrayIntrinsics;

namespace IntrinsicsPlayground.Benchmarks.Arrays
{
    public class ArrayIsSorted : ArrayBenchmarkBase
    {
        [Benchmark]
        public bool IsSorted_Simple() => IsSorted_Simple(ArrayOfInts);

        [Benchmark]
        public bool IsSorted_Simple_Optimized() => IsSorted_Simple2(ArrayOfInts);

        [Benchmark]
        public bool IsSorted_Sse41() => ArrayIntrinsics.IsSorted_Sse41(ArrayOfInts);

        [Benchmark(Baseline = true)]
        public bool IsSorted_Avx2() => ArrayIntrinsics.IsSorted_Avx2(ArrayOfInts);

        //[Benchmark]
        public bool IsSorted_LINQ()
        {
            // I am just kidding.. :)
            return ArrayOfInts.OrderBy(i => i).SequenceEqual(ArrayOfInts);
        }

        /*
        [Benchmark]
        public bool IsSorted_CppPinvoke()
        {
            return is_sorted_avx2_generic(ArrayOfInts, ArrayOfInts.Length);
        }

        [DllImport("NativeLib", CallingConvention = CallingConvention.Cdecl)]
        static extern bool is_sorted_avx2_generic(int[] array, int count);
        */

        // simple implementations to benchmark against intrinsics:

        public static bool IsSorted_Simple(int[] array)
        {
            if (array.Length < 2) return true;
            for (var i = 0; i < array.Length - 1; ++i)
            {
                if (array[i] > array[i + 1]) return false;
            }
            return true;
        }

        public static bool IsSorted_Simple2(int[] array)
        {
            if (array.Length < 2) return true;

            // Core idea by @consoleapp https://twitter.com/consoleapp/status/991380745234067458
            // Instead of accessing two elements each iteration, we only access the new
            // one and keep the old value around.

            // SAFETY: The loop is bounded by the array length.
            unsafe
            {
                var arrayLength = array.Length;
                fixed (int* ptr = array)
                {
                    var current = ptr[0];
                    for (var i = 1; i < arrayLength; ++i)
                    {
                        var next = ptr[i];
                        if (current > next) return false;
                        current = next;
                    }
                }
            }

            return true;
        }
    }
}
