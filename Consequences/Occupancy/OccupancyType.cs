using Numerics.Data;

namespace Consequences.Occupancy;

public class OccupancyType
{
    public required string Name { get; init; }

    public required OrderedPairedData StructureDamageFunction { get; init; }
    public required OrderedPairedData ContentDamageFunction { get; init; }

    public float FoundationHeightOffset { get; init; }

    public float StructureValuePercentageOfTheMean { get; init; } = 1.0f;
    public float ContentValuePercentageOfTheMean { get; init; } = 1.0f;

}
