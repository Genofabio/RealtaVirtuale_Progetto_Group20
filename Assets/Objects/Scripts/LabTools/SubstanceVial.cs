using System.Collections.Generic;
using UnityEngine;

public class SubstanceVial : MonoBehaviour, Pourable
{
    [SerializeField] private Substance substance;
    [SerializeField] private float maxVolume;

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
            liquid.SetFillSize(GetCurrentVolume() / maxVolume);
        }
    }

    private void OnValidate()
    {
        if (maxVolume <= 0)
        {
            Debug.LogWarning("maxVolume deve essere maggiore di 0. Fialetta piena di 10ml.");
            maxVolume = 10f;
        }
    }

    public void Pour(Fillable targetContainer, float amountToPour)
    {
        float totalAmount = GetCurrentVolume();
        if (totalAmount == 0 || amountToPour <= 0) return;
        if (amountToPour > totalAmount) amountToPour = totalAmount;

        List<Substance> pouredSubstance = new List<Substance>();
        pouredSubstance.Add(new Substance(substance.SubstanceName, amountToPour, substance.IsSolid));
        substance.Quantity -= amountToPour;
        SubstanceMixture pouredMix = new SubstanceMixture(pouredSubstance, false, -1);

        liquid.SetFillSize(GetCurrentVolume() / maxVolume);

        targetContainer.Fill(pouredMix);
    }

    public float GetCurrentVolume()
    {
        return substance.Quantity;
    }

    public SubstanceMixture PickUpVolume(float amountToExtract)
    {
        float totalAmount = GetCurrentVolume();
        SubstanceMixture extractedMix = new SubstanceMixture(new List<Substance>(), false, -1);
        if (totalAmount == 0 || amountToExtract <= 0) return extractedMix;
        if (amountToExtract > totalAmount) amountToExtract = totalAmount;

        List<Substance> extractedSubstance = new List<Substance>();
        extractedSubstance.Add(new Substance(substance.SubstanceName, amountToExtract, substance.IsSolid));
        substance.Quantity -= amountToExtract;

        extractedMix.Substances = extractedSubstance;

        liquid.SetFillSize(GetCurrentVolume() / maxVolume);

        return extractedMix;
    }

}
