using System.Runtime.Intrinsics.X86;

namespace IntrinsicsPlayground.Intrinsics.ArrayIntrinsics
{
    unsafe partial class ArrayIntrinsics
    {
        public static bool IsSorted_Sse41(int[] array)
        {
            if (array.Length < 2)
                return true;

            var i = 0;
            fixed (int* ptr = &array[0])
            {
                if (array.Length > 4)
                {
                    for (; i < array.Length - 4; i += 4)
                    {
                        var curr = Sse2.LoadVector128(ptr + i);
                        var next = Sse2.LoadVector128(ptr + i + 1);
                        var mask = Sse2.CompareGreaterThan(curr, next);

                        if (!Sse41.TestZ(mask, mask)) return false;
                    }
                }
            }

            for (; i < array.Length - 1; i++)
            {
                if (array[i] > array[i + 1]) return false;
            }
            return true;
        }

        public static bool IsSorted_Avx2(int[] array)
        {
            if (array.Length < 2)
                return true;

            fixed (int* ptr = &array[0])
            {
                var i = 0;
                if (array.Length > 8)
                {
                    for (; i < array.Length - 8; i += 8) //16 for AVX512
                    {
                        var curr = Avx.LoadVector256(ptr + i);
                        var next = Avx.LoadVector256(ptr + i + 1);
                        var mask = Avx2.CompareGreaterThan(curr, next);
                        if (!Avx.TestZ(mask, mask)) return false;
                    }
                }

                for (; i + 1 < array.Length; i++)
                {
                    if (array[i] > array[i + 1]) return false;
                }
            }
            return true;
        }
    }
}
