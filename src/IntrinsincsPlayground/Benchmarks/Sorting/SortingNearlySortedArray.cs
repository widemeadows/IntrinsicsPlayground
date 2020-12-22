using System.Linq;

namespace IntrinsicsPlayground.Benchmarks.Sorting
{
    public class SortingNearlySortedArray : SortBenchmarkBase
    {
        public override int[] CreateArray(int count)
        {
            var range = Enumerable.Range(0, count).ToArray();
            var nearlySortedAsc = range.ToArray();
            for (var i = 0; i < nearlySortedAsc.Length - 1; i += 7)
            {
                nearlySortedAsc[i] = 0;
            }

            return nearlySortedAsc;
        }
    }
}
