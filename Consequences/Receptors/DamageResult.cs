namespace Consequences.Receptors;

public readonly record struct DamageResult(
    double Structure,
    double Content)
{
    public const int ComponentCount = 2;

    public double Total => Structure + Content;
}
