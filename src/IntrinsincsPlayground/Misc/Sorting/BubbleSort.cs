namespace IntrinsicsPlayground.Misc.Sorting
{
    public static class BubbleSort
    {
        public static void Sort(int[] array)
        {
            for (var i = 0; i < array.Length; i++)
            {
                for (var j = i + 1; j < array.Length; j++)
                {
                    if (array[i] <= array[j]) continue;
                    var temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                }
            }
        }
    }
}
