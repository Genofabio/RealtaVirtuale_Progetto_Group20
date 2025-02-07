using System.Collections.Generic;
using UnityEngine;

public interface Pourable
{
    public void Pour(Fillable targetContainer, float amountToPour);

    public List<Substance> PickUpVolume(float amountToExtract);
}
