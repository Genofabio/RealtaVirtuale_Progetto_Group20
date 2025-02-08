using System.Collections.Generic;

public interface Pourable
{
    public void Pour(Fillable targetContainer, float amountToPour);

    public SubstancesMix PickUpVolume(float amountToExtract);
}
