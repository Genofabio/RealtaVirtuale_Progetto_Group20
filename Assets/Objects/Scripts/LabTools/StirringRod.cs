using UnityEngine;

public class StirringRod : MonoBehaviour
{
    public void Mix(Fillable contenitor)
    {
        contenitor.MixSubstances();
    }
}
