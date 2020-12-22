using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace IntrinsicsPlayground.Intrinsics.ArrayIntrinsics
{
    unsafe partial class ArrayIntrinsics
    {
        public static float Max_Avx(float[] array)
        {
            const int vecSize = 8;

            if (array.Length == 0) return 0;
            if (array.Length < vecSize) return Max_Soft(array, 0, float.MinValue);

            var i = 0;
            var max = Vector256.Create(float.MinValue);
            fixed (float* ptr = &array[0])
            {
                for (; i <= array.Length - vecSize; i += vecSize) //16 for AVX512
                {
                    var current = Avx.LoadVector256(ptr + i);
                    max = Avx.Max(current, max);
                }
            }

            var finalMax = ReduceMax(max);
            if (i < array.Length - 1) finalMax = Max_Soft(array, i, finalMax);

            return finalMax;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float Max_Soft(float[] array, int offset, float initialMax) // for small chunks
        {
            var max = initialMax;
            for (; offset < array.Length; offset++)
            {
                var item = array[offset];
                if (item > max) max = item;
            }
            return max;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float ReduceMax(Vector256<float> vector)
        {
            var hi128 = Avx.ExtractVector128(vector, 1);
            var lo128 = Avx.ExtractVector128(vector, 0);

            var hiTmp1 = Avx.Permute(hi128, 0x1b);
            var hiTmp2 = Avx.Permute(hi128, 0x4e);

            var loTmp1 = Avx.Permute(lo128, 0x1b);
            var loTmp2 = Avx.Permute(lo128, 0x4e);

            hi128 = Sse.Max(hi128, hiTmp1);
            hi128 = Sse.Max(hi128, hiTmp2);

            lo128 = Sse.Max(lo128, loTmp1);
            lo128 = Sse.Max(lo128, loTmp2);

            lo128 = Sse.Max(lo128, hi128);

            return lo128.GetElement(0);
        }
    }
}
