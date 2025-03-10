using System.Collections.Generic;
using UnityEngine;

public class PasteurPipe : MonoBehaviour, Dropper
{
    [SerializeField] private float capacity;

    private SubstanceMixture containedMixture;
    private bool full;

    private LiquidRenderer liquid;

    public float Capacity {
        get { return capacity; }
        set { capacity = value; }
    }

    private void Start()
    {
        liquid = GetComponentInChildren<LiquidRenderer>();
        if (liquid == null)
        {
            Debug.Log("Liquid NOT found");
        }
        else
        {
            liquid.SetFillSize(0);
        }

        containedMixture = new SubstanceMixture(new List<Substance>(), false, false, 0, 0, false, 0, -1, Color.clear, Color.clear);
    }

    public void Suck(Pourable source)
    {
        if (source == null || full) return;

        SubstanceMixture extractedMix = source.PickUpVolume(capacity, true);
        if (extractedMix.Substances.Count == 0 ) return;

        containedMixture.ExperimentStepReached = extractedMix.ExperimentStepReached;
        containedMixture.Mixed = extractedMix.Mixed;
        containedMixture.AddSubstanceMixture(extractedMix);

        full = true;
        liquid.SetFillSize(1);
        liquid.SetColor(extractedMix.MixtureLiquidColor);
    }

    public void Drop(Fillable targetContainer)
    {
        if (targetContainer == null || !full) return;

        float targetRemainingVolume = targetContainer.GetRemainingVolume();
        if (targetRemainingVolume < capacity) return;

        targetContainer.Fill(containedMixture);

        full = false;
        containedMixture = new SubstanceMixture(new List<Substance>(), false, false, 0, 0, false, 0, -1, Color.clear, Color.clear);
        liquid.SetFillSize(0);
        liquid.SetColor(Color.clear);
    }

    public bool IsFull()
    {
        return full;
    }
}
