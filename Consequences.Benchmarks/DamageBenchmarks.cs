using BenchmarkDotNet.Attributes;
using Consequences.Buildings;
using Consequences.Hazards;
using Consequences.Occupancy;
using Consequences.Receptors;

namespace Consequences.Benchmarks;

[MemoryDiagnoser]
public class DamageBenchmarks
{
    [Params(1_000, 100_000)]
    public int StructureCount;

    private Building[] _buildings = Array.Empty<Building>();
    private DepthVelocity[] _hazards = Array.Empty<DepthVelocity>();

    private float[] _depths = Array.Empty<float>();

    [GlobalSetup]
    public void Setup()
    {
        var occupancy = new OccupancyType
        {
            Name = "RES1",
            StructureDamageFunction = static d => Math.Clamp(d / 10f, 0f, 1f),
            ContentDamageFunction = static d => Math.Clamp(d / 8f, 0f, 1f),
            OtherDamageFunction = static _ => 0f,
            VehicleDamageFunction = static _ => 0f,
            FoundationHeightOffset = 0f,
        };

        var rng = new Random(42);
        _buildings = new Building[StructureCount];
        _hazards = new DepthVelocity[StructureCount];
        for (int i = 0; i < StructureCount; i++)
        {
            _buildings[i] = new Building
            {
                OccupancyType = occupancy,
                Value = (float)(100_000 + rng.NextDouble() * 200_000),
                ContentValue = (float)(50_000 + rng.NextDouble() * 100_000),
                FoundationHeight = (float)(rng.NextDouble() * 3.0),
                NumStories = 1,
                FloorHeight = 9f,
                AbleBodiedPeople = 2,
                LimitedMobilityPeople = 0,
            };
            _hazards[i] = new DepthVelocity(
                depth: (float)(rng.NextDouble() * 12.0),
                velocity: (float)(rng.NextDouble() * 5.0));
        }
        _depths = DoSampling();
    }

    public float[] DoSampling()
    {
        var rng = new Random(42);
        float[] res = new float[StructureCount];

        for (int i = 0; i < StructureCount; i++)
        {
            res[i] = (float)(rng.NextDouble() * 12.0);
        }
        return res;
    }

    [Benchmark(Baseline = true)]
    public float Alt1_Primitives()
    {
        float total = 0;
        var buildings = _buildings;
        for (int i = 0; i < buildings.Length; i++)
        {
            ref var b = ref buildings[i];
            total += b.Compute(_depths[i]);
        }
        return total;
    }


    [Benchmark]
    public float Alt2_GenerateHazard()
    {
        DepthHazard[] localHazards = new DepthHazard[StructureCount];
        for (int i = 0; i < StructureCount; i++)
        {
            localHazards[i] = new DepthHazard(_depths[i]);
        }

        float total = 0;
        var buildings = _buildings;
        for (int i = 0; i < buildings.Length; i++)
        {
            ref var b = ref buildings[i];
            total += b.Compute(localHazards[i]).Total;
        }
        return total;
    }

    [Benchmark]
    public float Alt3_ReuseHazard()
    {
        DepthVelocity localHazards = new();

        float total = 0;
        var buildings = _buildings;
        for (int i = 0; i < buildings.Length; i++)
        {
            ref var b = ref buildings[i];
            localHazards.Depth = _depths[i];
            total += b.Compute(localHazards).Total;
        }
        return total;
    }
    // [Benchmark]
    // public float Alt4_Concrete()
    // {
    //     for (int i = 0; i < StructureCount; i++)
    //     {
    //         var rng = new Random(42);
    //         _hazards[i] = new Hazard(
    //         depth: rng.NextDouble() * 12.0,
    //         velocity: rng.NextDouble() * 5.0,
    //         duration: 1.0);
    //     }

    //     float total = 0;
    //     var buildings = _buildings;
    //     var hazards = _hazards;
    //     for (int i = 0; i < buildings.Length; i++)
    //     {
    //         ref var b = ref buildings[i];
    //         total += b.ComputeComponentsConcrete(hazards[i]).Total;
    //     }
    //     return total;
    // }

    // [Benchmark]
    // public float Alt3_OutParams()
    // {
    //     float total = 0;
    //     var buildings = _buildings;
    //     var hazards = _hazards;
    //     for (int i = 0; i < buildings.Length; i++)
    //     {
    //         ref var b = ref buildings[i];
    //         total += b.Compute(hazards[i].Depth, hazards[i].Velocity, out _, out _);
    //     }
    //     return total;
    // }

    // [Benchmark]
    // public float Alt4_OutStruct()
    // {
    //     float total = 0;
    //     var buildings = _buildings;
    //     var hazards = _hazards;
    //     for (int i = 0; i < buildings.Length; i++)
    //     {
    //         ref var b = ref buildings[i];
    //         b.Compute(hazards[i].Depth, hazards[i].Velocity, out DamageResult result);
    //         total += result.Total;
    //     }
    //     return total;
    // }

    // [Benchmark]
    // public float Alt5_TotalOnly()
    // {
    //     float total = 0;
    //     var buildings = _buildings;
    //     var hazards = _hazards;
    //     for (int i = 0; i < buildings.Length; i++)
    //     {
    //         ref var b = ref buildings[i];
    //         total += b.Compute(hazards[i].Depth, hazards[i].Velocity);
    //     }
    //     return total;
    // }

    // [Benchmark]
    // public float Alt6_BatchedAPI()
    // {
    //     float total = 0;
    //     var results = BuildingBatch.ComputeBatch(_buildings, _hazards);
    //     for (int i = 0; i < results.Length; i++)
    //         total += results[i].Total;
    //     return total;
    // }

    // [Benchmark]
    // public float Alt7_BatchedTotalOnly()
    // {
    //     return BuildingBatch.ComputeBatchTotal(_buildings, _hazards);
    // }
}
