namespace Consequences.Hazards;

public struct DepthVelocity : IDepthVelocityHazard
{
    public float Depth { get; set; }
    public float Velocity { get; set; }
    public DepthVelocity(float depth, float velocity)
    {
        Depth = depth;
        Velocity = velocity;
    }
}
