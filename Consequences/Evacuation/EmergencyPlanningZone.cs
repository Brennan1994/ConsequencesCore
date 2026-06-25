using Numerics.Data;

namespace Consequences.Evacuation;

public class EmergencyPlanningZone
{
    public OrderedPairedData FirstAlertCdf { get; }
    public OrderedPairedData ProtectiveActionInitiationCdf { get; }
    public float WarningIssuanceOffsetMinutes { get; }
    public EmergencyPlanningZone(
        OrderedPairedData firstAlertCdf,
        OrderedPairedData protectiveActionInitiationCdf,
        float hazardIdentifiedMinutes,
        float communicationDelayMinutes,
        float warningIssuanceDelayMinutes)
    {
        FirstAlertCdf = firstAlertCdf;
        ProtectiveActionInitiationCdf = protectiveActionInitiationCdf;
        WarningIssuanceOffsetMinutes = hazardIdentifiedMinutes + communicationDelayMinutes + warningIssuanceDelayMinutes;
    }
}
