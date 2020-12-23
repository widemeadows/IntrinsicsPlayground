using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using BenchmarkDotNet.Attributes;

namespace IntrinsicsPlayground.Benchmarks.Intrinsics
{
    [DisassemblyDiagnoser]
    public class LeadingZeroCount
    {
        // benchmarking LZCNT against its soft implementation in Decimal https://github.com/dotnet/corert/blob/a4c6faac2c31bffc6f13287c6cb8c6a7bb9667fd/src/System.Private.CoreLib/src/System/Decimal.DecCalc.cs#L2047

        [Benchmark(Baseline = true)]
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public unsafe int[] LeadingZeroCount_HW()
        {
            var results = new int[100016];
            fixed (int* ptr = results)
            {
                for (uint i = 1; i < results.Length; i += 8)
                {
                    ptr[i] = (int) Lzcnt.LeadingZeroCount(i);
                    ptr[i + 1] = (int) Lzcnt.LeadingZeroCount(i + 1);
                    ptr[i + 2] = (int) Lzcnt.LeadingZeroCount(i + 2);
                    ptr[i + 3] = (int) Lzcnt.LeadingZeroCount(i + 3);
                    ptr[i + 4] = (int) Lzcnt.LeadingZeroCount(i + 4);
                    ptr[i + 5] = (int) Lzcnt.LeadingZeroCount(i + 5);
                    ptr[i + 6] = (int) Lzcnt.LeadingZeroCount(i + 6);
                    ptr[i + 7] = (int) Lzcnt.LeadingZeroCount(i + 7);
                }
            }

            return results;
        }

        [Benchmark]
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public unsafe int[] LeadingZeroCount_Avx2()
        {
            var results = new int[100016];
            fixed (int* ptr = results)
            {
                var maxBits = Vector256.Create(32);
                var bias = Vector256.Create(158).AsInt16();

                // https://stackoverflow.com/a/58827596/195651
                for (uint i = 0; i < results.Length; i += 8)
                {
                    var v = Vector256.Create(i, i + 1, i + 2, i + 3, i + 4, i + 5, i + 6, i + 7).AsInt32();

                    // prevent value from being rounded up to the next power of two
                    v = Avx2.AndNot(Avx2.ShiftRightLogical(v, 8), v); // keep 8 MSB

                    v = Avx.ConvertToVector256Single(v).AsInt32(); // convert an integer to float
                    v = Avx2.ShiftRightLogical(v, 23); // shift down the exponent

                    v = Avx2.SubtractSaturate(bias, v.AsInt16()).AsInt32(); // undo bias
                    v = Avx2.Min(v, maxBits); // clamp at 32

                    Avx.Store(&ptr[i], v);
                }

                // by convention
                ptr[0] = 0;
            }

            return results;
        }

        [Benchmark]
        public unsafe int[] LeadingZeroCount_Soft()
        {
            var results = new int[100016];
            fixed (int* ptr = results)
            {
                // see https://github.com/dotnet/corert/pull/5883#issuecomment-403617647
                for (uint i = 1; i < results.Length; i++)
                {
                    ptr[i] = LeadingZeroCount_SoftStatic(i);
                }
            }

            return results;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static int LeadingZeroCount_SoftStatic(uint value)
        {
            var c = 1;
            if ((value & 0xFFFF0000) == 0)
            {
                value <<= 16;
                c += 16;
            }
            if ((value & 0xFF000000) == 0)
            {
                value <<= 8;
                c += 8;
            }
            if ((value & 0xF0000000) == 0)
            {
                value <<= 4;
                c += 4;
            }
            if ((value & 0xC0000000) == 0)
            {
                value <<= 2;
                c += 2;
            }
            return c + ((int)value >> 31);
        }
    }
}
