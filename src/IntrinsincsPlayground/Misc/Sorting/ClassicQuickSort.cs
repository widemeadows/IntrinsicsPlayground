using System.Collections.Generic;

namespace IntrinsicsPlayground.Misc.Sorting
{
    public static class ClassicQuicksort
    {
        public static void Sort(IList<int> inputArray) => Sort(inputArray, 0, inputArray.Count - 1);

        private static void Sort(IList<int> array, int left, int right)
        {
            var pivot = array[(left + right) / 2];
            var originalLeft = left;
            var originalRight = right;

            // Partition
            do
            {
                // Skip elements already smaller or greater than the pivot.
                while (array[left] < pivot) ++left;
                while (pivot < array[right]) --right;
                if (left > right) break;

                array.SwapElements(left, right);
                ++left;
                --right;
            }
            while (left <= right);

            // Sort partitions
            if (originalLeft < right) Sort(array, originalLeft, right);
            if (left < originalRight) Sort(array, left, originalRight);
        }
    }
}
