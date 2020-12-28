using System.Collections.Generic;

namespace IntrinsicsPlayground.Misc.Sorting
{
    public static class Quicksort3Way
    {
        public static void Sort(IList<int> inputArray) => Sort(inputArray, 0, inputArray.Count - 1);

        private static void Sort(IList<int> array, int left, int right)
        {
            if (right <= left) return;

            var pivot = array[left]; // TODO: Why not array[(left + right) / 2] ?
            var originalLeft = left;
            var originalRight = right;
            var i = left + 1;

            // Partition
            while (i <= right)
            {
                var element = array[i];
                if (element < pivot)
                {
                    array.SwapElements(i, left);
                    ++i;
                    ++left;
                }
                else if (pivot < element)
                {
                    array.SwapElements(i, right);
                    --right;
                }
                else
                {
                    ++i;
                }
            }

            // Sort partitions
            if (originalLeft < left) Sort(array, originalLeft, left - 1);
            if (right < originalRight) Sort(array, right + 1, originalRight);
        }
    }
}
