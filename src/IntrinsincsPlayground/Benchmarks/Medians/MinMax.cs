using System;
using BenchmarkDotNet.Attributes;

namespace IntrinsicsPlayground.Benchmarks.Medians
{
    [DisassemblyDiagnoser]
    public class MinMax
    {
        private readonly Random _rand = new();
        private float _a;
        private float _b;

        [GlobalSetup]
        public void Setup()
        {
            var rand = _rand;
            _a = (float) (rand.NextDouble() * 2.0 - 1.0);
            _b = (float) (rand.NextDouble() * 2.0 - 1.0);
        }

        [Benchmark(Baseline = true)]
        public float MathMin() => Math.Min(_a, _b);

        [Benchmark]
        public float MathMin_Soft() => Misc.Median.MinMax.MathMin(_a, _b);

        [Benchmark]
        public float MathMinInlined() => Misc.Median.MinMax.MathMinInlined(_a, _b);

        [Benchmark]
        public float MinReorder() => Misc.Median.MinMax.MinReorder(_a, _b);

        [Benchmark]
        public float MinReorderWithMinScalar() => Misc.Median.MinMax.MinReorderWithMinScalar(_a, _b);

        [Benchmark]
        public float MinReorderWithMinScalar_Relaxed() => Misc.Median.MinMax.MinReorderWithMinScalar_Relaxed(_a, _b);

        [Benchmark]
        public float MinVectorized() => Misc.Median.MinMax.MinVectorized(_a, _b);
    }
}
