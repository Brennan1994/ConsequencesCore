namespace Consequences.Occupancy;

public class OccupancyType
{
    public required string Name { get; init; }

    public required Func<float, float> StructureDamageFunction { get; init; }
    public required Func<float, float> ContentDamageFunction { get; init; }
    public required Func<float, float> OtherDamageFunction { get; init; }
    public required Func<float, float> VehicleDamageFunction { get; init; }

    public float FoundationHeightOffset { get; init; }

    public float StructureValuePercentageOfTheMean { get; init; } = 1.0f;
    public float ContentValuePercentageOfTheMean { get; init; } = 1.0f;
    public float OtherValuePercentageOfTheMean { get; init; } = 1.0f;
    public float VehicleValuePercentageOfTheMean { get; init; } = 1.0f;
}
