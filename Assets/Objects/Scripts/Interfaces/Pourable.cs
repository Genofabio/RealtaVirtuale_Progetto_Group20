public interface Pourable
{
    void Pour(Fillable targetContainer, float amount);
    SubstanceMixture PickUpVolume(float amount);
    float GetCurrentVolume();
}
