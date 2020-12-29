using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace IntrinsicsPlayground.Misc.Median
{
    // https://gist.github.com/tannergooding/d81a6cd7530ec1cdc27e08530922f02a
    internal static class MinMax
    {
        public static float MathMin(float val1, float val2)
        {
            if ((val1 < val2) || float.IsNaN(val1))
            {
                return val1;
            }

            if (val1 == val2)
            {
                return float.IsNegative(val1) ? val1 : val2;
            }

            return val2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MathMinInlined(float val1, float val2)
        {
            if ((val1 < val2) || float.IsNaN(val1))
            {
                return val1;
            }

            if (val1 == val2)
            {
                return float.IsNegative(val1) ? val1 : val2;
            }

            return val2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinReorder(float val1, float val2)
        {
            if (val1 != val2)
            {
                if (!float.IsNaN(val1))
                {
                    return val1 < val2 ? val1 : val2;
                }

                return val1;
            }

            return float.IsNegative(val1) ? val1 : val2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinReorderWithMinScalar(float val1, float val2)
        {
            if (val1 != val2)
            {
                if (!float.IsNaN(val1))
                {
                    var lhs = Vector128.CreateScalarUnsafe(val1);
                    var rhs = Vector128.CreateScalarUnsafe(val2);
                    return Sse.MinScalar(lhs, rhs).ToScalar();
                }

                return val1;
            }

            return float.IsNegative(val1) ? val1 : val2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinReorderWithMinScalar_Relaxed(float val1, float val2)
        {
            // This version does not take care about NaN and ignores the difference between -0.0 and 0.0.
            var lhs = Vector128.CreateScalarUnsafe(val1);
            var rhs = Vector128.CreateScalarUnsafe(val2);
            return Sse.MinScalar(lhs, rhs).ToScalar();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinVectorized(float val1, float val2)
        {
            var lhs = Vector128.CreateScalarUnsafe(val1);
            var rhs = Vector128.CreateScalarUnsafe(val2);

            // Get the smaller of lhs or rhs, picking lhs if either is NaN or both are +/-0
            var smaller = Sse.MinScalar(rhs, lhs);

            // Compare lhs and rhs for equality, giving us a mask of all ones (equal) or all zeros (not-equal) and combine the raw bits of lhs and rhs
            var equalityMask = Sse.CompareScalarEqual(lhs, rhs);
            var combinedBits = Sse.Or(lhs, rhs);

            // AND the above two masks to either get a mask of all zeros (not-equal) or the combined bits (equal) and then combine that result with the smaller value.
            // This has the side-effect of setting the sign-bit if both were zero, but only one input was negative zero; and otherwise returning the original value.
            combinedBits = Sse.And(combinedBits, equalityMask);
            smaller = Sse.Or(smaller, combinedBits);

            // Check if rhs is NaN, giving us a mask of all ones (true) or all zeros (false)
            var nanMask = Sse.CompareScalarUnordered(rhs, rhs);

            // Select rhs if lhs was NaN; otherwise select smaller
            return Sse41.BlendVariable(smaller, rhs, nanMask).ToScalar();
        }
    }
}
