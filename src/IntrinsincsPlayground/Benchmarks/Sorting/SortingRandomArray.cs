using System;
using System.Linq;

namespace IntrinsicsPlayground.Benchmarks.Sorting
{
    public class SortingRandomArray : SortBenchmarkBase
    {
        private static readonly Random Random = new Random();
        private static int[] StaticRandomArray;

        public override int[] CreateArray(int count)
        {
            var array = PrepareRandomData();

            return array.Take(Count).ToArray();
        }

        private static int[] PrepareRandomData()
        {
            return StaticRandomArray ??= Enumerable.Range(0, 10000).OrderBy(i => Random.Next()).ToArray();
        }
    }
}
