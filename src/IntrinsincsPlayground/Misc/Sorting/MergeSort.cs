namespace IntrinsicsPlayground.Misc.Sorting
{
    public static class MergeSort
    {
        public static void Sort(int[] array) => Sort(array, 0, array.Length);

        private static void Sort(int[] array, int low, int high)
        {
            var n = high - low;
            if (n <= 1) return;

            var mid = low + n / 2;

            Sort(array, low, mid);
            Sort(array, mid, high);

            var aux = new int[n];
            var i = low;
            var j = mid;
            for (var k = 0; k < n; ++k)
            {
                if (i == mid)
                {
                    aux[k] = array[j++];
                }
                else if (j == high)
                {
                    aux[k] = array[i++];
                }
                else if (array[j].CompareTo(array[i]) < 0)
                {
                    aux[k] = array[j++];
                }
                else
                {
                    aux[k] = array[i++];
                }
            }

            for (var k = 0; k < n; ++k)
            {
                array[low + k] = aux[k];
            }
        }
    }
}
