using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace IntrinsicsPlayground.Misc.Median
{
    public static class Medians
    {
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public static float Median5_Soft2(float a, float b, float c, float d, float e)
        {
            // See https://github.com/dotnet/runtime/issues/33057
            var f = Math.Max(Math.Min(a, b), Math.Min(c, d));
            var g = Math.Min(Math.Max(a, b), Math.Max(c, d));
            return Median3_Soft2(e, f, g);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        public static float Median3_Soft2(float a, float b, float c) =>
            Math.Max(Math.Min(a, b), Math.Min(c, Math.Max(a, b)));

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public static Vector128<float> Median5_Relaxed2(Vector128<float> va, Vector128<float> vb, Vector128<float> vc, Vector128<float> vd, Vector128<float> ve)
        {
            // See https://github.com/dotnet/runtime/issues/33057
            var vf = Sse.Max(Sse.Min(va, vb), Sse.Min(vc, vd));
            var vg = Sse.Min(Sse.Max(va, vb), Sse.Max(vc, vd));

            return Sse.Max(Sse.Min(ve, vf), Sse.Min(vg, Sse.Max(ve, vf)));
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public static float Median5_Relaxed2(float a, float b, float c, float d, float e)
        {
            var va = Vector128.CreateScalarUnsafe(a);
            var vb = Vector128.CreateScalarUnsafe(b);
            var vc = Vector128.CreateScalarUnsafe(c);
            var vd = Vector128.CreateScalarUnsafe(d);
            var ve = Vector128.CreateScalarUnsafe(e);

            return Median5_Relaxed2(va, vb, vc, vd, ve).ToScalar();
        }

        // https://stackoverflow.com/a/481333/195651
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public static float Median5_Soft(float a, float b, float c, float d, float e)
        {
            // makes a < b and c < d
            Sort(ref a, ref b);
            Sort(ref c, ref d);

            // eliminate the lowest
            if (c < a)
            {
                Swap(ref b, ref d);
                c = a;
            }

            // gets e in
            a = e;

            // makes a < b
            Sort(ref a, ref b);

            // eliminate another lowest
            // remaining: a,b,d
            if (a < c)
            {
                Swap(ref b, ref d);
                a = c;
            }

            return Math.Min(d, a); // TODO: Could use relaxed min instead (ignoring NaN and -0.0 vs 0.0)
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static void Sort(ref float a, ref float b)
        {
            if (a <= b) return;
            Swap(ref a, ref b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static void Swap<T>(ref T a, ref T b) => (a, b) = (b, a);
    }
}
