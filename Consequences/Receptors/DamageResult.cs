namespace Consequences.Receptors;

public readonly record struct DamageResult(
    double Structure,
    double Content,
    double Other,
    double Vehicle)
{
    public const int ComponentCount = 4;

    public double Total => Structure + Content + Other + Vehicle;
}
