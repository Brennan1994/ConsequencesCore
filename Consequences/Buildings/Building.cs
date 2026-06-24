using Consequences.Hazards;
using Consequences.Occupancy;
using Consequences.Receptors;
using Consequences.Stability;

namespace Consequences.Buildings;

public struct Building : ICoreConsequenceReceptor
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

    private double EffectiveDepth(double depth) =>
        depth - FoundationHeight - OccupancyType.FoundationHeightOffset;

    // Alternative 1: one method per component.
    public double ComputeStructure(double depth)
    {
        var occ = OccupancyType;
        double value = Value * occ.StructureValuePercentageOfTheMean;
        return value * occ.StructureDamageFunction(EffectiveDepth(depth));
    }

    public double ComputeContent(double depth)
    {
        var occ = OccupancyType;
        double value = ContentValue * occ.ContentValuePercentageOfTheMean;
        return value * occ.ContentDamageFunction(EffectiveDepth(depth));
    }

    public double ComputeStructure(double depth, double velocity) => ComputeStructure(depth);
    public double ComputeContent(double depth, double velocity) => ComputeContent(depth);

    public double ComputeStructure(IHazard hazard) => ComputeStructure(hazard.Depth, hazard.Velocity);
    public double ComputeContent(IHazard hazard) => ComputeContent(hazard.Depth, hazard.Velocity);

    // Alternative 2: single struct return.
    public DamageResult ComputeComponents(double depth)
    {
        var occ = OccupancyType;
        double effectiveDepth = EffectiveDepth(depth);

        double structureValue = Value * occ.StructureValuePercentageOfTheMean;
        double contentValue = ContentValue * occ.ContentValuePercentageOfTheMean;

        return new DamageResult(
            structureValue * occ.StructureDamageFunction(effectiveDepth),
            contentValue * occ.ContentDamageFunction(effectiveDepth));
    }

    public DamageResult ComputeComponents(double depth, double velocity) => ComputeComponents(depth);
    public DamageResult ComputeComponents(IHazard hazard) => ComputeComponents(hazard.Depth, hazard.Velocity);

    // Alternative 3: out parameters per component, total as return value.
    public double Compute(double depth, out double content, out double structure)
    {
        var occ = OccupancyType;
        double effectiveDepth = EffectiveDepth(depth);

        double structureValue = Value * occ.StructureValuePercentageOfTheMean;
        double contentValue = ContentValue * occ.ContentValuePercentageOfTheMean;

        structure = structureValue * occ.StructureDamageFunction(effectiveDepth);
        content = contentValue * occ.ContentDamageFunction(effectiveDepth);
        return structure + content;
    }

    public double Compute(double depth, double velocity, out double content, out double structure) =>
        Compute(depth, out content, out structure);

    public double Compute(IHazard hazard, out double content, out double structure) =>
        Compute(hazard.Depth, hazard.Velocity, out content, out structure);

    // Alternative 4: caller-allocated DamageResult filled via out parameter.
    public void Compute(double depth, out DamageResult result)
    {
        var occ = OccupancyType;
        double effectiveDepth = EffectiveDepth(depth);

        double structureValue = Value * occ.StructureValuePercentageOfTheMean;
        double contentValue = ContentValue * occ.ContentValuePercentageOfTheMean;

        result = new DamageResult(
            structureValue * occ.StructureDamageFunction(effectiveDepth),
            contentValue * occ.ContentDamageFunction(effectiveDepth));
    }

    public void Compute(double depth, double velocity, out DamageResult result) =>
        Compute(depth, out result);

    public void Compute(IHazard hazard, out DamageResult result) =>
        Compute(hazard.Depth, hazard.Velocity, out result);
}
