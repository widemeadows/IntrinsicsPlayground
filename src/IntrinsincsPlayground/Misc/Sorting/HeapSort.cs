using System.Collections.Generic;

namespace IntrinsicsPlayground.Misc.Sorting
{
    public static class HeapSort
    {
        public static void Sort(IList<int> array)
        {
            var heapSize = array.Count;
            for (var p = (heapSize - 1) / 2; p >= 0; p--)
            {
                MaxHeapify(array, heapSize, p);
            }

            for (var i = array.Count - 1; i > 0; i--)
            {
                (array[i], array[0]) = (array[0], array[i]);

                heapSize--;
                MaxHeapify(array, heapSize, 0);
            }
        }

        private static void MaxHeapify(IList<int> input, int heapSize, int index)
        {
            var right = (index + 1) * 2;
            var left = right - 1;
            int largest;

            if (left < heapSize && input[left] > input[index])
            {
                largest = left;
            }
            else
            {
                largest = index;
            }

            if (right < heapSize && input[right] > input[largest])
            {
                largest = right;
            }

            if (largest != index)
            {
                (input[index], input[largest]) = (input[largest], input[index]);
                MaxHeapify(input, heapSize, largest);
            }
        }
    }
}
