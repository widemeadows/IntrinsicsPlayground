using System.Collections.Generic;

namespace IntrinsicsPlayground.Misc.Sorting
{
    public static class BubbleSort
    {
        public static void Sort(IList<int> array)
        {
            for (var i = 0; i < array.Count; ++i)
            for (var j = i + 1; j < array.Count; ++j)
            {
                if (array[i] <= array[j]) continue;
                (array[i], array[j]) = (array[j], array[i]);
            }
        }
    }
}
