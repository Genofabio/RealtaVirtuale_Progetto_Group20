public interface Fillable
{
    void Fill(SubstanceMixture mix);
    float GetRemainingVolume();
    void StirContents();
    bool CanContainLiquid();
    SubstanceMixture GetContainedSubstanceMixture();
}