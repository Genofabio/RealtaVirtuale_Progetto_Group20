using System.Collections.Generic;
using UnityEngine;

public class PasteurPipe : MonoBehaviour, Dropper
{
    [SerializeField] private SubstancesMix substancesMix;

    [SerializeField] private float capacity;
    private bool full;

    private LiquidRenderer liquid;

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
    }

    public void Suck(Pourable source)
    {
        if (source == null || full) return;

        SubstancesMix extractedMix = source.PickUpVolume(capacity);
        if (extractedMix.Substances.Count == 0 ) return;

        substancesMix.AddSubstancesMix(extractedMix);

        full = true;
        liquid.SetFillSize(1);
    }

    public void Drop(Fillable targetContainer)
    {
        if (targetContainer == null || !full) return;

        float targetRemainingVolume = targetContainer.GetRemainingVolume();
        if (targetRemainingVolume < capacity) return;

        targetContainer.Fill(substancesMix);

        full = false;
        substancesMix = new SubstancesMix(new List<Substance>(), false, -1);
        liquid.SetFillSize(0);
    }

    //private void AddSubstance(Substance substance)
    //{
    //    if (substance.Quantity <= 0) return;
    //    Substance existing = sub.Find(s => s.SubstanceName == substance.SubstanceName);
    //    if (existing != null)
    //    {
    //        existing.Quantity += substance.Quantity;
    //    }
    //    else
    //    {
    //        contents.Add(substance);
    //    }
    //}

    public bool IsFull()
    {
        return full;
    }
}
