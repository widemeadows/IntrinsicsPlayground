using System;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace IntrinsicsPlayground.Benchmarks.Sorting
{
    [MarkdownExporterAttribute.GitHub]
    [RPlotExporter]
    public abstract class SortBenchmarkBase
    {
        [Params(10, 100, 1000, 10000)]
        public int Count { get; set; }

        private int[] CurrentArray { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            CurrentArray = CreateArray(Count);
        }

        [Benchmark]
        public int[] ArraySort()
        {
            var array = CurrentArray.ToArray();
            Array.Sort(array);
            return array;
        }

        [Benchmark]
        public int[] Quicksort()
        {
            var array = CurrentArray.ToArray();
            Misc.Sorting.ClassicQuicksort.Sort(array);
            return array;
        }

        [Benchmark(Baseline = true)]
        public int[] DualPivotQuicksort()
        {
            var array = CurrentArray.ToArray();
            Misc.Sorting.JavaSort.Sort(array);
            return array;
        }

        [Benchmark]
        public int[] RadixSort()
        {
            var array = CurrentArray.ToArray();
            Misc.Sorting.RadixSort.Sort(array);
            return array;
        }

        public abstract int[] CreateArray(int count);
    }
}
