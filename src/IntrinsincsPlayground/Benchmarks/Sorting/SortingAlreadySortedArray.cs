using System.Linq;

namespace IntrinsicsPlayground.Benchmarks.Sorting
{
    public class SortingAlreadySortedArray : SortBenchmarkBase
    {
        public override int[] CreateArray(int count) => Enumerable.Range(0, count).ToArray();
    }
}
