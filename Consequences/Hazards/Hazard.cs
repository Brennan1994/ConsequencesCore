namespace Consequences.Hazards;

public struct Hazard : IHazard
{
    public Hazard(double depth, double velocity, double duration)
    {
        Depth = depth;
        Velocity = velocity;
        Duration = duration;
    }

    public double Depth { get; set;}
    public double Velocity { get; set; }
    public double Duration { get; set;}
}
