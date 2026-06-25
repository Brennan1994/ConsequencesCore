namespace Consequences.Receptors;

public readonly record struct DamageResult(
    float Structure,
    float Content)
{
    public const int ComponentCount = 2;

    public float Total => Structure + Content;
}
