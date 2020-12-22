namespace IntrinsicsPlayground.Misc.Sorting
{
    public static class InsertionSort
    {
        public static void Sort(int[] array)
        {
            for (var i = 0; i < array.Length - 1; ++i)
            for (var j = i + 1; j > 0; --j)
            {
                if (array[j - 1] <= array[j]) continue;
                (array[j - 1], array[j]) = (array[j], array[j - 1]);
            }
        }
    }
}
