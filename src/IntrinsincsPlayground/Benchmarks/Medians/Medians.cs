using System;
using System.Linq;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace IntrinsicsPlayground.Benchmarks.Medians
{
    [DisassemblyDiagnoser]
    public class Medians
    {
        private int[] _unsortedArray;

        [GlobalSetup]
        public void Setup()
        {
            var rand = new Random();
            _unsortedArray = Enumerable
                .Range(0, 1024)
                .OrderBy(i => rand.Next())
                .ToArray();
        }

        [Benchmark(Baseline = true)]
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public float Median5_Soft()
        {
            var array = _unsortedArray;
            return Misc.Median.Medians.Median5_Soft(array[0], array[1], array[2], array[3], array[4]);
        }

        [Benchmark]
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public float Median5_Soft2()
        {
            var array = _unsortedArray;
            return Misc.Median.Medians.Median5_Soft2(array[0], array[1], array[2], array[3], array[4]);
        }

        [Benchmark]
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public float Median5_Relaxed2()
        {
            var array = _unsortedArray;
            return Misc.Median.Medians.Median5_Relaxed2(array[0], array[1], array[2], array[3], array[4]);
        }
    }
}
