using Consequences.Evacuation;
using Consequences.LifeLossEnums;
using Consequences.Occupancy;
using Consequences.Stability;

namespace Consequences.Buildings;

public readonly struct LifeLossBuilding
{
    public required Building Building { get; init; }
    public float AtticHeight { get; init; }
    public float OtherFloorHeight { get; init; }
    public float GroundFloorHeight { get; init; }
    public int AbleBodiedPeople { get; init; }
    public int LimitedMobilityPeople { get; init; }

    public void GenerateEvacGroups(Random random, EmergencyPlanningZone epz, EvacuationParameters parameters, int buildingIndex, List<EvacuationGroup> result)
    {
        if (AbleBodiedPeople <= 0 && LimitedMobilityPeople <= 0) return;
        OccupancyType occtype = Building.OccupancyType;

        int tempAble = AbleBodiedPeople;
        int tempLimited = LimitedMobilityPeople;
        double maxMobRate = epz.ProtectiveActionInitiationCdf[^1].Y;
        float timeToWarned;
        float timeToMobilized;

        if (occtype.CollectivelyWarned && occtype.CollectivelyMobilize)
        {
            timeToWarned = (float)(epz.FirstAlertCdf.GetXFromY(random.NextDouble()) + epz.WarningIssuanceOffsetMinutes);
            timeToMobilized = GetTimeToMobilized(timeToWarned, maxMobRate, random.NextDouble(), epz);
            // Offset successive vehicles entering the road so they don't all stack on the same position.
            // 4 seconds in minutes ≈ 0.0667.
            const double timeMobilizedOffset = 0.066666667;
            int counter = 0;
            while (tempAble != 0 || tempLimited != 0)
            {
                var g = GenerateGroup(tempAble, tempLimited, random, parameters, buildingIndex, timeToWarned, (float)(timeToMobilized + timeMobilizedOffset * counter));
                tempAble -= g.Under65;
                tempLimited -= g.Over65;
                result.Add(g);
                counter += 1;
            }
        }
        else if (occtype.CollectivelyWarned && !occtype.CollectivelyMobilize)
        {
            timeToWarned = (float)(epz.FirstAlertCdf.GetXFromY(random.NextDouble()) + epz.WarningIssuanceOffsetMinutes);
            while (tempAble != 0 || tempLimited != 0)
            {
                timeToMobilized = GetTimeToMobilized(timeToWarned, maxMobRate, random.NextDouble(), epz);
                var g = GenerateGroup(tempAble, tempLimited, random, parameters, buildingIndex, timeToWarned, timeToMobilized);
                tempAble -= g.Under65;
                tempLimited -= g.Over65;
                result.Add(g);
            }
        }
        else if (!occtype.CollectivelyWarned && !occtype.CollectivelyMobilize)
        {
            while (tempAble != 0 || tempLimited != 0)
            {
                timeToWarned = (float)(epz.FirstAlertCdf.GetXFromY(random.NextDouble()) + epz.WarningIssuanceOffsetMinutes);
                timeToMobilized = GetTimeToMobilized(timeToWarned, maxMobRate, random.NextDouble(), epz);
                var g = GenerateGroup(tempAble, tempLimited, random, parameters, buildingIndex, timeToWarned, timeToMobilized);
                tempAble -= g.Under65;
                tempLimited -= g.Over65;
                result.Add(g);
            }
        }
        else
        {
            // Single mobilize time for the building, but per-group warning times.
            timeToMobilized = GetTimeToMobilized(0, maxMobRate, random.NextDouble(), epz);
            while (tempAble != 0 || tempLimited != 0)
            {
                timeToWarned = (float)(epz.FirstAlertCdf.GetXFromY(random.NextDouble()) + epz.WarningIssuanceOffsetMinutes);
                var g = GenerateGroup(tempAble, tempLimited, random, parameters, buildingIndex, timeToWarned, timeToMobilized);
                tempAble -= g.Under65;
                tempLimited -= g.Over65;
                result.Add(g);
            }
        }
    }

    private static EvacuationGroup GenerateGroup(int tempAble,
     int tempLimited,
     Random random,
     EvacuationParameters parameters,
     int buildingIndex,
     float timeToWarned,
     float timeToMobilized)
    {
        byte popAble = 0;
        byte popLimited = 0;
        byte vehicleCapacity = parameters.VehicleOccupancyRate;

        if (tempAble + tempLimited <= vehicleCapacity)
        {
            popAble = (byte)tempAble;
            popLimited = (byte)tempLimited;
        }
        else if (tempLimited == 0)
        {
            popAble = vehicleCapacity;
        }
        else if (tempAble == 0)
        {
            popLimited = vehicleCapacity;
        }
        else
        {
            while (popLimited + popAble < vehicleCapacity)
            {
                if (tempLimited - popLimited > 0)
                {
                    popLimited += 1;
                    if (popLimited + popAble == vehicleCapacity) break;
                }
                if (tempAble - popAble > 0) popAble += 1;
            }
        }

        bool hasGPS = random.NextDouble() < parameters.FractionTrafficReroute;
        TransportationMode transportationMode = TransportationMode.LowClearanceVehicle;
        StabilityCriteria groupStability = parameters.LowClearanceStability;
        float depthThreshold = 2f;

        if (parameters.SimulateTraffic)
        {
            if (random.NextDouble() < parameters.FractionInVehicles)
            {
                if (random.NextDouble() < parameters.FractionInCars)
                {
                    transportationMode = TransportationMode.LowClearanceVehicle;
                    groupStability = parameters.LowClearanceStability;
                    depthThreshold = (float)Math.Max(parameters.LowClearanceRoadEntryCdf.GetXFromY(random.NextDouble()), 0);
                }
                else
                {
                    transportationMode = TransportationMode.HighClearanceVehicle;
                    groupStability = parameters.HighClearanceStability;
                    depthThreshold = (float)Math.Max(parameters.HighClearanceRoadEntryCdf.GetXFromY(random.NextDouble()), 0);
                }
            }
            else
            {
                transportationMode = TransportationMode.Foot;
                groupStability = parameters.PedestrianStability;
            }
        }

        return new EvacuationGroup(popAble, popLimited, transportationMode, groupStability, depthThreshold, buildingIndex, timeToWarned, timeToMobilized, hasGPS);
    }

    private static float GetTimeToMobilized(float timeToWarned, double maxFractionMobilized, double randomNumber, EmergencyPlanningZone epz)
    {
        if (randomNumber > maxFractionMobilized) return float.MinValue;
        return (float)(timeToWarned + epz.ProtectiveActionInitiationCdf.GetXFromY(randomNumber));
    }
}
