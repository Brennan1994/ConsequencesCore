using Consequences.Hazards;

namespace Consequences.Receptors;

public interface ICoreConsequenceReceptor
{
    //Alternative 1 
    double ComputeStructure(IHazard hazard);
    double ComputeContent(IHazard hazard);
    double ComputeStructure(double depth);
    double ComputeContent(double depth);

    double ComputeStructure(double depth, double velocity);
    double ComputeContent(double depth, double velocity);

    //Alternative 2 
    DamageResult ComputeComponents(IHazard hazard);
    DamageResult ComputeComponents(double depth);
    DamageResult ComputeComponents(double depth, double velocity);

    // Components-via-buffer variant for benchmarking against the DamageResult return.
    // Caller supplies a Span sized to ComponentCount; method writes the per-component
    // damages in DamageResult field order (Structure, Content, Other, Vehicle) and
    // returns the total.
    double Compute(IHazard hazard, out double content, out double structure);
    double Compute(double depth, out double content, out double structure);
    double Compute(double depth, double velocity, out double content, out double structure);
}
