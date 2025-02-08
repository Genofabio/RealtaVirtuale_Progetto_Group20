using System.Collections.Generic;

public interface Pourable
{
    public void Pour(Fillable targetContainer, float amountToPour);

    public List<Substance> PickUpVolume(float amountToExtract);

    public float GetCurrentVolume();
}
