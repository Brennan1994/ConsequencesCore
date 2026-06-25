using BenchmarkDotNet.Attributes;

namespace Consequences.Benchmarks;

// Minimal reproduction of the interface-argument-vs-generic-argument cost we
// hit in Building.ComputeComponents. The shape mirrors the real call site:
//   - a value-type "payload" implementing a small interface
//   - a hot loop that reads two properties off the payload and does cheap math
// The only thing that varies across benchmarks is *how the payload is passed*.
[MemoryDiagnoser]
public class InterfaceVsGenericBenchmarks
{
    public interface ISample
    {
        float A { get; }
        float B { get; }
    }

    public struct SampleStruct : ISample
    {
        public float A { get; set; }
        public float B { get; set; }
    }

    [Params(1_000, 100_000)]
    public int N;

    private SampleStruct[] _samples = Array.Empty<SampleStruct>();

    [GlobalSetup]
    public void Setup()
    {
        var rng = new Random(42);
        _samples = new SampleStruct[N];
        for (int i = 0; i < N; i++)
        {
            _samples[i] = new SampleStruct { A = (float)rng.NextDouble(), B = (float)rng.NextDouble() };
        }
    }

    // Lower bound: no abstraction at all. The JIT sees the concrete struct
    // and inlines the property reads.
    [Benchmark(Baseline = true)]
    public float ViaConcrete()
    {
        float total = 0;
        var samples = _samples;
        for (int i = 0; i < samples.Length; i++)
        {
            total += Concrete(samples[i]);
        }
        return total;
    }

    // Generic constraint. The JIT specializes Generic<SampleStruct> for the
    // value-type T, so property access stays direct — no boxing, no vtable.
    [Benchmark]
    public float ViaGeneric()
    {
        float total = 0;
        var samples = _samples;
        for (int i = 0; i < samples.Length; i++)
        {
            total += Generic(samples[i]);
        }
        return total;
    }

    // Interface parameter. Passing a struct as ISample boxes it on every call
    // and turns the property reads into virtual interface dispatches.
    [Benchmark]
    public float ViaInterface()
    {
        float total = 0;
        var samples = _samples;
        for (int i = 0; i < samples.Length; i++)
        {
            total += Interface(samples[i]);
        }
        return total;
    }

    private static float Concrete(SampleStruct s) => s.A * 2f + s.B;

    private static float Generic<T>(T s) where T : ISample => s.A * 2f + s.B;

    private static float Interface(ISample s) => s.A * 2f + s.B;
}
