// Copyright license of the original code:
//
//     Copyright (c) 2009, 2011, Oracle and/or its affiliates. All rights reserved.
//     DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
//
//     This code is free software; you can redistribute it and/or modify it
//     under the terms of the GNU General Public License version 2 only, as
//     published by the Free Software Foundation.  Oracle designates this
//     particular file as subject to the "Classpath" exception as provided
//     by Oracle in the LICENSE file that accompanied this code.
//
//     This code is distributed in the hope that it will be useful, but WITHOUT
//     ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
//     FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
//     version 2 for more details (a copy is included in the LICENSE file that
//     accompanied this code).
//
//     You should have received a copy of the GNU General Public License version
//     2 along with this work; if not, write to the Free Software Foundation,
//     Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
//
//     Please contact Oracle, 500 Oracle Parkway, Redwood Shores, CA 94065 USA
//     or visit www.oracle.com if you need additional information or have any
//     questions.

using System;
using System.Collections.Generic;

namespace IntrinsicsPlayground.Misc.Sorting
{
    /// <summary>
    ///     A dual-pivot quicksort that falls back to a mergesort in trivial cases.
    /// </summary>
    /// <remarks>
    ///     The implementation was copied from
    ///     https://github.com/openjdk-mirror/jdk7u-jdk/blob/master/src/share/classes/java/util/DualPivotQuicksort.java
    ///     and converted to C# just for research purposes.
    /// </remarks>
    public static class JavaSort
    {
        private const int MaxRunCount = 67;
        private const int MaxRunLength = 33;
        private const int QuicksortThreshold = 286;
        private const int InsertionSortThreshold = 47;

        public static void Sort(int[] a) => Sort(a, 0, a.Length - 1, null, 0, 0);

        private static void Sort(int[] a, int left, int right, int[] work, int workBase, int workLen)
        {
            // Use Sort on small arrays
            if (right - left < QuicksortThreshold)
            {
                Sort(a, left, right, true);
                return;
            }

            // Index run[i] is the start of i-th run
            // (ascending or descending sequence).
            var run = new int[MaxRunCount + 1];
            var count = 0;
            run[0] = left;

            // Check if the array is nearly sorted
            for (var k = left; k < right; run[count] = k)
            {
                if (a[k] < a[k + 1])
                {
                    // ascending
                    while (++k <= right && a[k - 1] <= a[k]) ;
                }
                else if (a[k] > a[k + 1])
                {
                    // descending
                    while (++k <= right && a[k - 1] >= a[k]) ;
                    for (int lo = run[count] - 1, hi = k; ++lo < --hi;)
                    {
                        SwapElements(a, lo, hi);
                    }
                }
                else
                {
                    // equal
                    for (var m = MaxRunLength; ++k <= right && a[k - 1] == a[k];)
                    {
                        if (--m != 0) continue;
                        Sort(a, left, right, true);
                        return;
                    }
                }

                // The array is not highly structured,
                // use Sort instead of merge sort.
                if (++count != MaxRunCount) continue;
                Sort(a, left, right, true);
                return;
            }

            // Check special cases
            // Implementation note: variable "right" is increased by 1.
            if (run[count] == right++)
            {
                // The last run contains one element
                run[++count] = right;
            }
            else if (count == 1)
            {
                // The array is already sorted
                return;
            }

            // Determine alternation base for merge
            byte odd = 0;
            for (var n = 1; (n <<= 1) < count; odd ^= 1) ;

            // Use or create temporary array b for merging
            int[] b; // temp array; alternates with a
            int aOffset, bOffset; // array offsets from 'left'
            var bLen = right - left; // space needed for b
            if (work == null || workLen < bLen || workBase + bLen > work.Length)
            {
                work = new int[bLen];
                workBase = 0;
            }

            if (odd == 0)
            {
                Array.Copy(a, left, work, workBase, bLen);
                b = a;
                bOffset = 0;
                a = work;
                aOffset = workBase - left;
            }
            else
            {
                b = work;
                aOffset = 0;
                bOffset = workBase - left;
            }

            // Merging
            for (int last; count > 1; count = last)
            {
                for (var k = (last = 0) + 2; k <= count; k += 2)
                {
                    var hi = run[k];
                    var mi = run[k - 1];
                    for (int i = run[k - 2], p = i, q = mi; i < hi; ++i)
                    {
                        if (q >= hi || p < mi && a[p + aOffset] <= a[q + aOffset])
                        {
                            b[i + bOffset] = a[p++ + aOffset];
                        }
                        else
                        {
                            b[i + bOffset] = a[q++ + aOffset];
                        }
                    }

                    run[++last] = hi;
                }

                if ((count & 1) != 0)
                {
                    for (int i = right, lo = run[count - 1];
                        --i >= lo;
                        b[i + bOffset] = a[i + aOffset]
                    ) ;
                    run[++last] = right;
                }

                SwapValues(ref a, ref b);
                SwapValues(ref aOffset, ref bOffset);
            }
        }
        private static void Sort(IList<int> a, int left, int right, bool leftmost)
        {
            var length = right - left + 1;

            // Use insertion sort on tiny arrays
            if (length < InsertionSortThreshold)
            {
                SortWithInsertionSort(a, left, right, leftmost);
                return;
            }

            // Inexpensive approximation of length / 7
            var seventh = (length >> 3) + (length >> 6) + 1;

            // Sort five evenly spaced elements around (and including) the
            // center element in the range. These elements will be used for
            // pivot selection as described below. The choice for spacing
            // these elements was empirically determined to work well on
            // a wide variety of inputs.
            var e3 = (int) ((uint) (left + right) >> 1); // The midpoint
            var e2 = e3 - seventh;
            var e1 = e2 - seventh;
            var e4 = e3 + seventh;
            var e5 = e4 + seventh;

            // Sort these elements using insertion sort
            InsertionSortPivotCandidates(a, e1, e2, e3, e4, e5);

            var allPivotCandidatesAreDifferent = a[e1] != a[e2] && a[e2] != a[e3] && a[e3] != a[e4] && a[e4] != a[e5];
            if (allPivotCandidatesAreDifferent)
            {
                SortWithDualPivotPartition(a, left, right, e1, e2, e4, e5, leftmost);
            }
            else
            {
                SortWithSinglePivotPartition(a, left, right, e3, leftmost);
            }
        }

        private static void SortWithDualPivotPartition(IList<int> a, int left, int right, int e1, int e2, int e4, int e5, bool leftmost)
        {
            // Pointers
            var less = left; // The index of the first element of center part
            var great = right; // The index before the first element of right part

            // Use the second and fourth of the five sorted elements as pivots.
            // These values are inexpensive approximations of the first and
            // second terciles of the array. Note that pivot1 <= pivot2.
            var pivot1 = a[e2];
            var pivot2 = a[e4];

            // The first and the last elements to be sorted are moved to the
            // locations formerly occupied by the pivots. When partitioning
            // is complete, the pivots are swapped back into their final
            // positions, and excluded from subsequent sorting.
            a[e2] = a[left];
            a[e4] = a[right];

            // Skip elements, which are already less or greater than pivot values.
            while (a[++less] < pivot1);
            while (a[--great] > pivot2) ;

            // Partitioning:
            //
            //   left part           center part                   right part
            // +--------------------------------------------------------------+
            // |  < pivot1  |  pivot1 <= && <= pivot2  |    ?    |  > pivot2  |
            // +--------------------------------------------------------------+
            //              ^                          ^       ^
            //              |                          |       |
            //             less                        k     great
            //
            // Invariants:
            //
            //              all in (left, less)   < pivot1
            //    pivot1 <= all in [less, k)     <= pivot2
            //              all in (great, right) > pivot2
            //
            // Pointer k is the first index of ?-part.
            outer1:
            for (var k = less - 1; ++k <= great;)
            {
                var ak = a[k];
                if (ak < pivot1)
                {
                    // Move a[k] to left part
                    a[k] = a[less];

                    // Here and below we use "a[i] = b; i++;" instead
                    // of "a[i++] = b;" due to performance issue.
                    a[less] = ak;
                    ++less;
                }
                else if (ak > pivot2)
                {
                    // Move a[k] to right part
                    while (a[great] > pivot2)
                    {
                        if (great-- == k)
                        {
                            goto outer1;
                        }
                    }

                    if (a[great] < pivot1)
                    {
                        // a[great] <= pivot2
                        a[k] = a[less];
                        a[less] = a[great];
                        ++less;
                    }
                    else
                    {
                        // pivot1 <= a[great] <= pivot2
                        a[k] = a[great];
                    }

                    // Here and below we use "a[i] = b; i--;" instead
                    // of "a[i--] = b;" due to performance issue.
                    a[great] = ak;
                    --great;
                }
            }

            // Swap pivots into their final positions
            a[left] = a[less - 1];
            a[less - 1] = pivot1;
            a[right] = a[great + 1];
            a[great + 1] = pivot2;

            // Sort left and right parts recursively, excluding known pivots
            Sort(a, left, less - 2, leftmost);
            Sort(a, great + 2, right, false);

            // If center part is too large (comprises > 4/7 of the array),
            // swap internal pivot values to ends.
            if (less < e1 && e5 < great)
            {
                // Skip elements, which are equal to pivot values.
                while (a[less] == pivot1)
                {
                    ++less;
                }

                while (a[great] == pivot2)
                {
                    --great;
                }

                // Partitioning:
                //
                //   left part         center part                  right part
                // +----------------------------------------------------------+
                // | == pivot1 |  pivot1 < && < pivot2  |    ?    | == pivot2 |
                // +----------------------------------------------------------+
                //             ^                        ^       ^
                //             |                        |       |
                //            less                      k     great
                //
                // Invariants:
                //
                //              all in (*,  less) == pivot1
                //     pivot1 < all in [less,  k)  < pivot2
                //              all in (great, *) == pivot2
                //
                // Pointer k is the first index of ?-part.
                outer2:
                for (var k = less - 1; ++k <= great;)
                {
                    var ak = a[k];
                    if (ak == pivot1)
                    {
                        // Move a[k] to left part
                        a[k] = a[less];
                        a[less] = ak;
                        ++less;
                    }
                    else if (ak == pivot2)
                    {
                        // Move a[k] to right part
                        while (a[great] == pivot2)
                        {
                            if (great-- == k)
                            {
                                goto outer2;
                            }
                        }

                        if (a[great] == pivot1)
                        {
                            // a[great] < pivot2
                            a[k] = a[less];

                            // Even though a[great] equals to pivot1, the
                            // assignment a[less] = pivot1 may be incorrect,
                            // if a[great] and pivot1 are floating-point zeros
                            // of different signs. Therefore in float and
                            // double sorting methods we have to use more
                            // accurate assignment a[less] = a[great].
                            a[less] = pivot1;
                            ++less;
                        }
                        else
                        {
                            // pivot1 < a[great] < pivot2
                            a[k] = a[great];
                        }

                        a[great] = ak;
                        --great;
                    }
                }
            }

            // Sort center part recursively
            Sort(a, less, great, false);
        }

        private static void SortWithSinglePivotPartition(IList<int> a, int left, int right, int e3, bool leftmost)
        {
            // Pointers
            var less = left; // The index of the first element of center part
            var great = right; // The index before the first element of right part

            // Partitioning with one pivot
            //
            // Use the third of the five sorted elements as pivot.
            // This value is inexpensive approximation of the median.
            var pivot = a[e3];

            // Partitioning degenerates to the traditional 3-way
            // (or "Dutch National Flag") schema:
            //
            //   left part    center part              right part
            // +-------------------------------------------------+
            // |  < pivot  |   == pivot   |     ?    |  > pivot  |
            // +-------------------------------------------------+
            //              ^              ^        ^
            //              |              |        |
            //             less            k      great
            //
            // Invariants:
            //
            //   all in (left, less)   < pivot
            //   all in [less, k)     == pivot
            //   all in (great, right) > pivot
            //
            // Pointer k is the first index of ?-part.
            for (var k = less; k <= great; ++k)
            {
                if (a[k] == pivot)
                {
                    continue;
                }

                var ak = a[k];
                if (ak < pivot)
                {
                    // Move a[k] to left part
                    a[k] = a[less];
                    a[less] = ak;
                    ++less;
                }
                else
                {
                    // a[k] > pivot - Move a[k] to right part
                    while (a[great] > pivot)
                    {
                        --great;
                    }

                    if (a[great] < pivot)
                    {
                        // a[great] <= pivot
                        a[k] = a[less];
                        a[less] = a[great];
                        ++less;
                    }
                    else
                    {
                        // a[great] == pivot
                        //
                        // Even though a[great] equals to pivot, the
                        // assignment a[k] = pivot may be incorrect,
                        // if a[great] and pivot are floating-point
                        // zeros of different signs. Therefore in float
                        // and double sorting methods we have to use
                        // more accurate assignment a[k] = a[great].
                        a[k] = pivot;
                    }

                    a[great] = ak;
                    --great;
                }
            }

            // Sort left and right parts recursively.
            // All elements from center part are equal
            // and, therefore, already sorted.
            Sort(a, left, less - 1, leftmost);
            Sort(a, great + 1, right, false);
        }

        private static void SortWithInsertionSort(IList<int> a, int left, int right, bool leftmost)
        {
            if (leftmost)
            {
                // Traditional (without sentinel) insertion sort, optimized for (Java) server VM,
                // is used in case of the leftmost part.
                for (int i = left, j = i; i < right; j = ++i)
                {
                    var element = a[i + 1];
                    while (element < a[j])
                    {
                        a[j + 1] = a[j];
                        if (j-- == left)
                        {
                            break;
                        }
                    }

                    a[j + 1] = element;
                }
            }
            else
            {
                // Skip the longest ascending sequence.
                do
                {
                    if (left >= right)
                    {
                        return;
                    }
                } while (a[++left] >= a[left - 1]);

                // Every element from adjoining part plays the role
                // of sentinel, therefore this allows us to avoid the
                // left range check on each iteration. Moreover, we use
                // the more optimized algorithm, so called pair insertion
                // sort, which is faster (in the context of Sort)
                // than traditional implementation of insertion sort.
                for (var k = left; ++left <= right; k = ++left)
                {
                    var a1 = a[k];
                    var a2 = a[left];

                    if (a1 < a2)
                    {
                        a2 = a1;
                        a1 = a[left];
                    }

                    while (a1 < a[--k])
                    {
                        a[k + 2] = a[k];
                    }

                    a[++k + 1] = a1;

                    while (a2 < a[--k])
                    {
                        a[k + 1] = a[k];
                    }

                    a[k + 1] = a2;
                }

                var last = a[right];

                while (last < a[--right])
                {
                    a[right + 1] = a[right];
                }

                a[right + 1] = last;
            }
        }

        private static void InsertionSortPivotCandidates(IList<int> a, int e1, int e2, int e3, int e4, int e5)
        {
            if (a[e2] < a[e1])
            {
                var t = a[e2];
                a[e2] = a[e1];
                a[e1] = t;
            }

            if (a[e3] < a[e2])
            {
                var t = a[e3];
                a[e3] = a[e2];
                a[e2] = t;
                if (t < a[e1])
                {
                    a[e2] = a[e1];
                    a[e1] = t;
                }
            }

            if (a[e4] < a[e3])
            {
                var t = a[e4];
                a[e4] = a[e3];
                a[e3] = t;
                if (t < a[e2])
                {
                    a[e3] = a[e2];
                    a[e2] = t;
                    if (t < a[e1])
                    {
                        a[e2] = a[e1];
                        a[e1] = t;
                    }
                }
            }

            if (a[e5] < a[e4])
            {
                var t = a[e5];
                a[e5] = a[e4];
                a[e4] = t;
                if (t < a[e3])
                {
                    a[e4] = a[e3];
                    a[e3] = t;
                    if (t < a[e2])
                    {
                        a[e3] = a[e2];
                        a[e2] = t;
                        if (t < a[e1])
                        {
                            a[e2] = a[e1];
                            a[e1] = t;
                        }
                    }
                }
            }
        }

        private static void SwapElements(IList<int> a, int left, int right) => (a[left], a[right]) = (a[right], a[left]);

        private static void SwapValues<T>(ref T left, ref T right) => (left, right) = (right, left);
    }
}
