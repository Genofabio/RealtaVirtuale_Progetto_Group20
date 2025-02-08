using System.Collections.Generic;
using UnityEngine;

public interface Fillable
{
    public void Fill(SubstancesMix mix);
    public float GetRemainingVolume();
    public void MixSubstances();
}