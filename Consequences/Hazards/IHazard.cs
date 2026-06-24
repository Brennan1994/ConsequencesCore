namespace Consequences.Hazards;

public interface IHazard
{
    double Depth { get; set;}
    double Velocity { get; set;}
    double Duration { get; set;}
}
