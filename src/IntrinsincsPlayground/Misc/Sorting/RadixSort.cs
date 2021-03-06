using System;

namespace IntrinsicsPlayground.Misc.Sorting
{
    public static class RadixSort
    {
        // Copied from https://en.wikibooks.org/wiki/Algorithm_Implementation/Sorting/Radix_sort
        public static void Sort(int[] a)
        {
            // our helper array
            var t = new int[a.Length];

            // number of bits our group will be long
            var r = 4; // TODO: try to set this also to 2, 8 or 16 to see if it is quicker or not

            // number of bits of a C# int
            var b = 32;

            // counting and prefix arrays
            // (note dimensions 2^r which is the number of all possible values of a r-bit number)
            var count = new int[1 << r];
            var pref = new int[1 << r];

            // number of groups
            var groups = (int)Math.Ceiling(b / (double)r);

            // the mask to identify groups
            var mask = (1 << r) - 1;

            // the algorithm:
            for (int c = 0, shift = 0; c < groups; c++, shift += r)
            {
                // reset count array
                for (var j = 0; j < count.Length; ++j)
                {
                    count[j] = 0;
                }

                // counting elements of the c-th group
                for (var i = 0; i < a.Length; ++i)
                {
                    count[(a[i] >> shift) & mask]++;
                }

                // calculating prefixes
                pref[0] = 0;
                for (var i = 1; i < count.Length; ++i)
                {
                    pref[i] = pref[i - 1] + count[i - 1];
                }

                // from a[] to t[] elements ordered by c-th group
                for (var i = 0; i < a.Length; ++i)
                {
                    t[pref[(a[i] >> shift) & mask]++] = a[i];
                }

                // a[]=t[] and start again until the last group
                t.CopyTo(a, 0);
            }
            // a is sorted
        }
    }
}
