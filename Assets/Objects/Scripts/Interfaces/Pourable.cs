public interface Pourable
{
    void Pour(Fillable targetContainer, float amount);
    SubstanceMixture PickUpVolume(float amount, bool onlyLiquid);
    float GetCurrentVolume();
    public SubstanceMixture GetContainedSubstanceMixture();
}
