namespace Consequences.Hazards;

public struct DepthHazard : IDepthHazard
{
    public float Depth { get; set; }

    public DepthHazard(float depth)
    {
        Depth = depth;
    }
}
