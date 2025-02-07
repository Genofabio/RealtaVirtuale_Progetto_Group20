using System.Collections.Generic;
using UnityEngine;

public interface Fillable
{
    public void Fill(List<Substance> substances);
    public float GetRemainingVolume();
}