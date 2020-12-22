using System;
using System.Runtime.Intrinsics.X86;
using BenchmarkDotNet.Running;

namespace IntrinsicsPlayground
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            if (!Sse41.IsSupported || !Avx2.IsSupported)
                throw new NotSupportedException(":(");

            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }
}
