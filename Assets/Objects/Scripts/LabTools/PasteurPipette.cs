using System.Collections.Generic;
using UnityEngine;

public class PasteurPipe : MonoBehaviour, Dropper
{
    private List<Substance> contents = new List<Substance>();

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

        List<Substance> extractedSubstances = source.PickUpVolume(capacity);
        if (extractedSubstances.Count == 0 ) return;

        foreach (var sub in extractedSubstances)
        {
            AddSubstance(sub);
        }

        full = true;
        liquid.SetFillSize(1);
    }

    public void Drop(Fillable targetContainer)
    {
        if (targetContainer == null || !full) return;

        float targetRemainingVolume = targetContainer.GetRemainingVolume();
        if (targetRemainingVolume < capacity) return;

        targetContainer.Fill(contents);

        full = false;
        contents = new List<Substance>();
        liquid.SetFillSize(0);
    }

    private void AddSubstance(Substance substance)
    {
        if (substance.Quantity <= 0) return;
        Substance existing = contents.Find(s => s.SubstanceName == substance.SubstanceName);
        if (existing != null)
        {
            existing.Quantity += substance.Quantity;
        }
        else
        {
            contents.Add(substance);
        }
    }

    public bool IsFull()
    {
        return full;
    }
}
