using Consequences.Hazards;
using Consequences.LifeLossEnums;
using Consequences.Stability;

namespace Consequences.Evacuation;

public class EvacuationGroup
{
    public byte Under65 { get; }
    public byte Over65 { get; }
    public int OriginIndex { get; }

    public float WarningTime { get; }
    public float InitialMobilizeTime { get; }

    public TransportationMode ModeOfTransportation { get; }
    public float DepthThreshold { get; }
    public StabilityCriteria StabilityCriteria { get; }
    public bool HasGPS { get; }

    public int GroupIndex { get; set; }
    public float ActualMobilizeTime { get; set; }
    public bool Warned { get; set; }
    public bool Mobilized { get; set; }
    public bool Safe { get; set; }
    public bool CaughtEvacuating { get; set; }

    public int TotalPopulation => Under65 + Over65;

    public EvacuationGroup(
        byte under65,
        byte over65,
        TransportationMode modeOfTransportation,
        StabilityCriteria stabilityCriteria,
        float depthThreshold,
        int originIndex,
        float warningTime,
        float initialMobilizeTime,
        bool hasGPS)
    {
        Under65 = under65;
        Over65 = over65;
        ModeOfTransportation = modeOfTransportation;
        StabilityCriteria = stabilityCriteria;
        DepthThreshold = depthThreshold;
        OriginIndex = originIndex;
        WarningTime = warningTime;
        InitialMobilizeTime = initialMobilizeTime;
        HasGPS = hasGPS;
        ActualMobilizeTime = initialMobilizeTime;
    }

    public bool StabilityLost(float depth, float velocity, float foundationHeight = 0f)
    {
        if (depth <= 0) return false;
        return StabilityCriteria.Collapsed(depth, velocity, foundationHeight);
    }

    public bool StabilityLost(HydraulicTimeSeries hydraulics, float foundationHeight = 0f)
    {
        if (hydraulics.MaxDepth <= 0) return false;
        return StabilityCriteria.Collapsed(hydraulics, foundationHeight);
    }
}
