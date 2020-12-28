using System.Collections.Generic;

namespace IntrinsicsPlayground
{
    internal static class ArrayExtensions
    {
        public static void SwapElements<T>(this IList<T> array, int left, int right) =>
            (array[left], array[right]) = (array[right], array[left]);
    }
}
