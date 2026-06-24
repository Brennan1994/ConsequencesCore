using Consequences.Buildings;
using Consequences.Hazards;
using Consequences.Occupancy;
using Consequences.Stability;

static double Linear(double depth) => Math.Clamp(depth / 10.0, 0.0, 1.0);

var residential = new OccupancyType
{
    Name = "RES1-1S-NB",
    StructureDamageFunction = Linear,
    ContentDamageFunction = d => Math.Clamp(d / 8.0, 0.0, 1.0),
    OtherDamageFunction = d => Math.Clamp(d / 12.0, 0.0, 1.0),
    VehicleDamageFunction = d => d > 1.0 ? 1.0 : 0.0,
    FoundationHeightOffset = 0.0,
};

var structure = new Building
{
    OccupancyType = residential,
    Value = 200_000,
    ContentValue = 100_000,
    FoundationHeight = 1.5,
    NumStories = 1,
    FloorHeight = 9.0,
    AbleBodiedPeople = 3,
    LimitedMobilityPeople = 1,
    SampledStabilityCriteria = StabilityCriteria.DepthVelocityProduct(threshold: 6.0),
};

var hazards = new Hazard[]
{
    new(depth: 2.0, velocity: 1.0, duration: 1.0),
    new(depth: 6.0, velocity: 2.0, duration: 2.0),
    new(depth: 12.0, velocity: 4.0, duration: 3.0),
};

Console.WriteLine($"Building: {structure.OccupancyType.Name}");
Console.WriteLine($"  Values: S=${structure.Value:N0} C=${structure.ContentValue:N0}");
Console.WriteLine($"  FoundationHeight={structure.FoundationHeight} ft");
Console.WriteLine();

foreach (var hazard in hazards)
{
    // Alt 1: per-component method calls
    double altStructure = structure.ComputeStructure(hazard);
    double altContent = structure.ComputeContent(hazard);
    double altTotal = altStructure + altContent;

    // Alt 2: struct return
    var components = structure.ComputeComponents(hazard);

    // Alt 3: out-param + total return
    double outTotal = structure.Compute(hazard, out double outContent, out double outStructure);

    bool stable = structure.SampledStabilityCriteria!.Evaluate(hazard);

    Console.WriteLine($"Hazard depth={hazard.Depth} ft, vel={hazard.Velocity} ft/s, dur={hazard.Duration} hr");
    Console.WriteLine($"  Alt 1 per-method   : total=${altTotal:N2}  [S=${altStructure:N2} C=${altContent:N2}]");
    Console.WriteLine($"  Alt 2 struct       : total=${components.Total:N2}  [S=${components.Structure:N2} C=${components.Content:N2}]");
    Console.WriteLine($"  Alt 3 out-params   : total=${outTotal:N2}  [S=${outStructure:N2} C=${outContent:N2}]");
    Console.WriteLine($"  Stable (d*v < 6.0) : {stable}");
    Console.WriteLine();
}
