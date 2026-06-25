using Consequences.Stability;
using Numerics.Data;

namespace Consequences.Evacuation;

public class EvacuationParameters
{
    public bool CollectivelyWarned { get; init; }
    public bool CollectivelyMobilize { get; init; }
    public byte VehicleOccupancyRate { get; init; } = 1;
    public float FractionInVehicles { get; init; }
    public float FractionInCars { get; init; }

    public bool SimulateTraffic { get; init; }
    public float FractionTrafficReroute { get; init; }

    public required StabilityCriteria LowClearanceStability { get; init; }
    public required StabilityCriteria HighClearanceStability { get; init; }
    public required StabilityCriteria PedestrianStability { get; init; }

    public required OrderedPairedData LowClearanceRoadEntryCdf { get; init; }
    public required OrderedPairedData HighClearanceRoadEntryCdf { get; init; }
}
