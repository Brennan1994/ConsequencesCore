using Consequences.Hazards;
using Consequences.Occupancy;
using Consequences.Receptors;
using Consequences.Stability;

namespace Consequences.Buildings;

public struct Building 
{
    public required OccupancyType OccupancyType { get; init; }

    public float Value { get; init; }
    public float ContentValue { get; init; }

    public float FoundationHeight { get; init; }
    public int NumStories { get; init; }
    public float FloorHeight { get; init; }

    public int AbleBodiedPeople { get; init; }
    public int LimitedMobilityPeople { get; init; }

    public StabilityCriteria? SampledStabilityCriteria { get; init; }

    // Alternative 2: single struct return.
    public static DamageResult Compute(float depth, Building building)
    {
        var occ = building.OccupancyType;
        float effectiveDepth = depth - building.FoundationHeight - building.OccupancyType.FoundationHeightOffset;

        float structureValue = building.Value * occ.StructureValuePercentageOfTheMean;
        float contentValue = building.ContentValue * occ.ContentValuePercentageOfTheMean;

        return new DamageResult(
            structureValue * occ.StructureDamageFunction(effectiveDepth),
            contentValue * occ.ContentDamageFunction(effectiveDepth));
    }

    public static DamageResult Compute(float depth, float velocity, Building building)  {
     //   if(building.SampledStabilityCriteria.Collapsed(depth, velocity, building.FoundationHeight ))

        var occ = building.OccupancyType;
        float effectiveDepth = depth - building.FoundationHeight - building.OccupancyType.FoundationHeightOffset;

        float structureValue = building.Value * occ.StructureValuePercentageOfTheMean;
        float contentValue = building.ContentValue * occ.ContentValuePercentageOfTheMean;

        return new DamageResult(
            structureValue * occ.StructureDamageFunction(effectiveDepth),
            contentValue * occ.ContentDamageFunction(effectiveDepth));
    }

    public readonly DamageResult Compute<THazard>(THazard hazard) where THazard : IDepthHazard =>
        Compute(hazard.Depth, this);

    // Alternative 5: total only, no components surfaced ****Fastest possible. 
    public readonly float Compute(float depth)
    {
        var occ = OccupancyType;
        float effectiveDepth = depth - FoundationHeight - OccupancyType.FoundationHeightOffset;

        float structureValue = Value * occ.StructureValuePercentageOfTheMean;
        float contentValue = ContentValue * occ.ContentValuePercentageOfTheMean;

        return structureValue * occ.StructureDamageFunction(effectiveDepth)
             + contentValue * occ.ContentDamageFunction(effectiveDepth);
    }
}
