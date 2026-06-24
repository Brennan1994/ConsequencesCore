using Consequences.Hazards;
using Consequences.Occupancy;
using Consequences.Receptors;
using Consequences.Stability;

namespace Consequences.Buildings;

public struct Building : ICoreConsequenceReceptor<DamageResult>
{
    public required OccupancyType OccupancyType { get; init; }

    public double Value { get; init; }
    public double ContentValue { get; init; }

    public double FoundationHeight { get; init; }
    public int NumStories { get; init; }
    public double FloorHeight { get; init; }

    public int AbleBodiedPeople { get; init; }
    public int LimitedMobilityPeople { get; init; }

    public StabilityCriteria? SampledStabilityCriteria { get; init; }

    // Alternative 2: single struct return.
    public readonly DamageResult ComputeComponents(double depth)
    {
        var occ = OccupancyType;
        double effectiveDepth = depth - FoundationHeight - OccupancyType.FoundationHeightOffset;

        double structureValue = Value * occ.StructureValuePercentageOfTheMean;
        double contentValue = ContentValue * occ.ContentValuePercentageOfTheMean;

        return new DamageResult(
            structureValue * occ.StructureDamageFunction(effectiveDepth),
            contentValue * occ.ContentDamageFunction(effectiveDepth));
    }

    public readonly DamageResult ComputeComponents(double depth, double velocity)  {
        var occ = OccupancyType;
        double effectiveDepth = depth - FoundationHeight - OccupancyType.FoundationHeightOffset;

        double structureValue = Value * occ.StructureValuePercentageOfTheMean;
        double contentValue = ContentValue * occ.ContentValuePercentageOfTheMean;

        return new DamageResult(
            structureValue * occ.StructureDamageFunction(effectiveDepth),
            contentValue * occ.ContentDamageFunction(effectiveDepth));
    }
    public readonly DamageResult ComputeComponents<THazard>(THazard hazard) where THazard : IHazard =>
        ComputeComponents(hazard.Depth, hazard.Velocity);
    public readonly DamageResult ComputeComponentsGenerics<THazard>(THazard hazard) where THazard : IHazard
    {
        var occ = OccupancyType;
        double effectiveDepth = hazard.Depth - FoundationHeight - OccupancyType.FoundationHeightOffset;

        double structureValue = Value * occ.StructureValuePercentageOfTheMean;
        double contentValue = ContentValue * occ.ContentValuePercentageOfTheMean;

        return new DamageResult(
            structureValue * occ.StructureDamageFunction(effectiveDepth),
            contentValue * occ.ContentDamageFunction(effectiveDepth));
    }

       public readonly DamageResult ComputeComponentsConcrete(Hazard hazard)
    {
        var occ = OccupancyType;
        double effectiveDepth = hazard.Depth - FoundationHeight - OccupancyType.FoundationHeightOffset;

        double structureValue = Value * occ.StructureValuePercentageOfTheMean;
        double contentValue = ContentValue * occ.ContentValuePercentageOfTheMean;

        return new DamageResult(
            structureValue * occ.StructureDamageFunction(effectiveDepth),
            contentValue * occ.ContentDamageFunction(effectiveDepth));
    }


    // Alternative 3: out parameters per component, total as return value.
    public readonly double Compute(double depth, out double content, out double structure)
    {
        var occ = OccupancyType;
        double effectiveDepth = depth - FoundationHeight - OccupancyType.FoundationHeightOffset;

        double structureValue = Value * occ.StructureValuePercentageOfTheMean;
        double contentValue = ContentValue * occ.ContentValuePercentageOfTheMean;

        structure = structureValue * occ.StructureDamageFunction(effectiveDepth);
        content = contentValue * occ.ContentDamageFunction(effectiveDepth);
        return structure + content;
    }

    public readonly double Compute(double depth, double velocity, out double content, out double structure) =>
        Compute(depth, out content, out structure);

    public readonly double Compute<THazard>(THazard hazard, out double content, out double structure) where THazard : IHazard =>
        Compute(hazard.Depth, hazard.Velocity, out content, out structure);

    // Alternative 4: caller-allocated DamageResult filled via out parameter.
    public readonly void Compute(double depth, out DamageResult result)
    {
        var occ = OccupancyType;
        double effectiveDepth = depth - FoundationHeight - OccupancyType.FoundationHeightOffset;

        double structureValue = Value * occ.StructureValuePercentageOfTheMean;
        double contentValue = ContentValue * occ.ContentValuePercentageOfTheMean;

        result = new DamageResult(
            structureValue * occ.StructureDamageFunction(effectiveDepth),
            contentValue * occ.ContentDamageFunction(effectiveDepth));
    }

    public readonly void Compute(double depth, double velocity, out DamageResult result) =>
        Compute(depth, out result);

    public readonly void Compute<THazard>(THazard hazard, out DamageResult result) where THazard : IHazard =>
        Compute(hazard.Depth, hazard.Velocity, out result);

    // Alternative 5: total only, no components surfaced.
    public readonly double Compute(double depth)
    {
        var occ = OccupancyType;
        double effectiveDepth = depth - FoundationHeight - OccupancyType.FoundationHeightOffset;

        double structureValue = Value * occ.StructureValuePercentageOfTheMean;
        double contentValue = ContentValue * occ.ContentValuePercentageOfTheMean;

        return structureValue * occ.StructureDamageFunction(effectiveDepth)
             + contentValue * occ.ContentDamageFunction(effectiveDepth);
    }

    public readonly double Compute(double depth, double velocity) => Compute(depth);
    public readonly double Compute<THazard>(THazard hazard) where THazard : IHazard => Compute(hazard.Depth, hazard.Velocity);
}
