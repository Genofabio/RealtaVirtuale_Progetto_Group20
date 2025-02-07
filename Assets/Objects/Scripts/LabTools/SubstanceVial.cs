using System.Collections.Generic;
using UnityEngine;

public class SubstanceVial : MonoBehaviour, Pourable
{
    [SerializeField] private Substance substance;
    [SerializeField] private float maxVolume;
    private LiquidRenderer liquid;

    private void OnValidate()
    {
        if (maxVolume <= 0)
        {
            Debug.LogWarning("maxVolume deve essere maggiore di 0. Fialetta piena di 10ml.");
            maxVolume = 10f;
        }
    }

    void Start()
    {
        if (maxVolume <= 0)
        {
            Debug.LogWarning("maxVolume deve essere maggiore di 0. Fialetta piena di 10ml.");
            maxVolume = 10f;
        }

        liquid = GetComponentInChildren<LiquidRenderer>();
        if (liquid == null)
        {
            Debug.Log("Liquid NOT found");
        }
        else
        {
            liquid.SetFillSize(GetCurrentVolume() / maxVolume);
        }
    }

    public float GetCurrentVolume()
    {
        return substance.Quantity;
    }

    public List<Substance> PickUpVolume(float amountToExtract)
    {
        float totalAmount = GetCurrentVolume();
        if (totalAmount == 0 || amountToExtract <= 0) return new List<Substance>();
        if (amountToExtract > totalAmount) amountToExtract = totalAmount;

        List<Substance> extractedSubstance = new List<Substance>();
        extractedSubstance.Add(new Substance(substance.SubstanceName, amountToExtract));
        substance.Quantity -= amountToExtract;

        liquid.SetFillSize(GetCurrentVolume() / maxVolume);

        return extractedSubstance;
    }

    public void Pour(Fillable targetContainer, float amountToPour)
    {
        float totalAmount = GetCurrentVolume();
        if (totalAmount == 0 || amountToPour <= 0) return;
        if (amountToPour > totalAmount) amountToPour = totalAmount;

        List<Substance> pouredSubstance = new List<Substance>();
        pouredSubstance.Add(new Substance(substance.SubstanceName, amountToPour));
        substance.Quantity -= amountToPour;

        liquid.SetFillSize(GetCurrentVolume() / maxVolume);

        // Versa nel becher di destinazione
        targetContainer.Fill(pouredSubstance);
    }


}
