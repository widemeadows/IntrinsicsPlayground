﻿using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace IntrinsicsPlayground.Intrinsics.ArrayIntrinsics
{
    unsafe partial class ArrayIntrinsics
    {
        public static bool Contains_Avx2(int[] array, int element)
        {
            if (array.Length < 1) return false;

            fixed (int* ptr = &array[0])
            {
                var i = 0;
                if (array.Length > 8 * 2)
                {
                    var elementVec = Vector256.Create(element);
                    for (; i < array.Length - 8; i += 8) //16 for AVX512
                    {
                        var curr = Avx.LoadVector256(ptr + i);
                        var mask = Avx2.CompareEqual(curr, elementVec);
                        if (!Avx.TestZ(mask, mask))
                            return true;
                    }
                }

                for (; i < array.Length; i++)
                {
                    if (array[i] == element) return true;
                }
            }
            return false;
        }
    }
}
