using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SubstanceMixture
{
    [SerializeField] private List<Substance> substances = new List<Substance>();
    [SerializeField] private Color mixtureColor;

    [SerializeField] private bool mixed = false;

    [SerializeField] private int experimentStepReached = -1;

    public SubstanceMixture(List<Substance> substances, bool mixed, int experimentStepReached, Color mixtureColor)
    {
        Substances = substances.Select(substance => substance.Clone()).ToList();
        Mixed = mixed;
        ExperimentStepReached = experimentStepReached;
        MixtureColor = mixtureColor;
    }

    public List<Substance> Substances
    {
        get { return substances.Select(substance => substance.Clone()).ToList(); }
        set { substances = value.Select(substance => substance.Clone()).ToList(); }
    }

    public bool Mixed
    {
        get { return mixed; }
        set { mixed = value; }
    }

    public int ExperimentStepReached
    {
        get { return experimentStepReached; }
        set { experimentStepReached = value; }
    }

    public Color MixtureColor 
    { 
        get { return mixtureColor; }
        set { mixtureColor = value; }
    }

    public SubstanceMixture Clone()
    {
        return new SubstanceMixture(new List<Substance>(this.Substances), this.Mixed, this.ExperimentStepReached, this.MixtureColor);
    }

    public float GetCurrentVolume()
    {
        float sum = 0f;
        foreach (var substance in substances)
        {
            sum += substance.Quantity;
        }
        return sum;
    }

    public float GetLiquidVolume()
    {
        float sum = 0f;
        foreach (var substance in substances)
        {
            if (!substance.IsSolid)
            {
                sum += substance.Quantity;
            }
        }
        return sum;
    }

    public float GetSolidVolume()
    {
        float sum = 0f;
        foreach (var substance in substances)
        {
            if (substance.IsSolid)
            {
                sum += substance.Quantity;
            }
        }
        return sum;
    }

    public void AddSubstanceMixture(SubstanceMixture mix) 
    {
        if (mix.Mixed && ((Mixed && HasSameSubstancePercentageForAll(mix)) || GetCurrentVolume() < 0.1))
        {
            Mixed = true;
        } 
        else
        {
            Mixed = false;
        }

        float totalVolume = GetCurrentVolume() + mix.GetCurrentVolume();
        Color newMixtureColor = CalculateMixtureColor(mix, totalVolume);

        foreach (var substance in mix.Substances)
        {
            AddSubstance(substance);
        }

        MixtureColor = newMixtureColor;
    }

    private void AddSubstance(Substance newSubstance)
    {
        if (newSubstance.Quantity <= 0) return;
        float totalAmount = GetCurrentVolume() + newSubstance.Quantity;

        Substance existing = substances.Find(s => s.SubstanceName == newSubstance.SubstanceName);
        if (existing != null)
        {
            existing.Quantity += newSubstance.Quantity;
        }
        else
        {
            substances.Add(newSubstance);
        }
    }

    private Color CalculateMixtureColor(SubstanceMixture mix, float totalVolume)
    {
        if (totalVolume <= 0) return Color.clear; 

        Color currentColor = MixtureColor * GetCurrentVolume();
        Color addedColor = mix.MixtureColor * mix.GetCurrentVolume();

        return (currentColor + addedColor) / totalVolume;
    }

    public void StirSubstances()
    {
        mixed = true;
    }

    public List<Substance> ExtractSubstances(float amountToPour)
    {
        float totalAmount = GetCurrentVolume();
        List<Substance> pouredSubstances = new List<Substance>();

        foreach (var sub in substances)
        {
            float pouredAmount = (sub.Quantity / totalAmount) * amountToPour;
            if (pouredAmount > 0)
            {
                pouredSubstances.Add(new Substance(sub.SubstanceName, pouredAmount, sub.IsSolid));
                sub.Quantity -= pouredAmount;
            }
        }

        substances.RemoveAll(sub => sub.Quantity <= 0.001);

        if (!(substances.Count > 0)) Mixed = false;

        return pouredSubstances;
    }

    public bool IsSimilarTo(SubstanceMixture mix)
    {
        if (mix.mixed == true && mixed != mix.mixed) return false;

        // Controllo che ogni sostanza in mix sia presente in contents con esattamente la stessa quantità
        foreach (Substance sostanzaMix in mix.substances)
        {
            Substance sostanzaInBecher = substances.Find(s => s.SubstanceName == sostanzaMix.SubstanceName);
            if (sostanzaInBecher == null || sostanzaInBecher.Quantity < sostanzaMix.Quantity - sostanzaMix.Quantity / 20 || sostanzaInBecher.Quantity > sostanzaMix.Quantity + sostanzaMix.Quantity / 20)
            {
                return false;
            }
        }

        // Controllo che contents non abbia sostanze extra oltre a quelle in mix
        foreach (Substance sostanzaInBecher in substances)
        {
            if (!mix.substances.Any(s => s.SubstanceName == sostanzaInBecher.SubstanceName))
            {
                return false;
            }
        }

        return true;
    }

    public bool HasSameSubstancePercentage(SubstanceMixture mix)
    {
        if (mix.substances.Count == 0 || substances.Count == 0) return false;

        // Filtra solo le sostanze che sono presenti in entrambi i mix
        var commonSubstances = mix.substances.Where(ms => substances.Any(bs => bs.SubstanceName == ms.SubstanceName)).ToList();
        if (commonSubstances.Count == 0) return false;

        // Calcola le quantità totali considerando solo le sostanze comuni
        float totalQuantityMix = commonSubstances.Sum(s => s.Quantity);
        float totalQuantityBecher = substances.Where(s => commonSubstances.Any(m => m.SubstanceName == s.SubstanceName))
                                              .Sum(s => s.Quantity);
        if (totalQuantityMix < 0.001 || totalQuantityBecher < 0.001) return false;

        // Controlla che le percentuali siano coerenti
        foreach (Substance substance in commonSubstances)
        {
            Substance becherSubstance = substances.FirstOrDefault(s => s.SubstanceName == substance.SubstanceName);
            if (becherSubstance == null) continue;

            double percentageMix = (substance.Quantity / totalQuantityMix) * 100;
            double percentageBecher = (becherSubstance.Quantity / totalQuantityBecher) * 100;

            if (Math.Abs(percentageMix - percentageBecher) > 4) return false;  
        }

        return true;
    }

    public bool HasSameSubstancePercentageForAll(SubstanceMixture mix)
    {
        if (mix.substances.Count == 0 || substances.Count == 0) return false;

        float totalQuantityMix = mix.substances.Sum(s => s.Quantity);
        float totalQuantityBecher = substances.Sum(s => s.Quantity);

        if (totalQuantityMix < 0.001 || totalQuantityBecher < 0.001) return false;

        foreach (Substance substanceInMix in mix.substances)
        {
            Substance correspondingSubstanceInBecher = substances.FirstOrDefault(s => s.SubstanceName == substanceInMix.SubstanceName);

            if (correspondingSubstanceInBecher != null)
            {
                double percentageMix = (substanceInMix.Quantity / totalQuantityMix) * 100;
                double percentageBecher = (correspondingSubstanceInBecher.Quantity / totalQuantityBecher) * 100;

                if (Math.Abs(percentageMix - percentageBecher) > 4)
                    return false;
            }
            else
            {
                // Se la sostanza non è presente in substances, la percentuale deve essere circa zero
                if (!substances.Any(s => s.SubstanceName == substanceInMix.SubstanceName))
                {
                    double percentageMix = (substanceInMix.Quantity / totalQuantityBecher) * 100;

                    if (Math.Abs(percentageMix) > 1)
                        return false;
                }
            }
        }

        foreach (Substance substanceInBecher in substances)
        {
            // Se la sostanza non è presente in mix, la percentuale deve essere circa zero
            if (!mix.substances.Any(s => s.SubstanceName == substanceInBecher.SubstanceName))
            {
                double percentageBecher = (substanceInBecher.Quantity / totalQuantityBecher) * 100;

                if (Math.Abs(percentageBecher) > 1)
                    return false;
            }
        }

        return true;
    }

    public bool CanBecome(SubstanceMixture mix)
    {
        foreach (Substance mixSubstance in mix.substances)
        {
            Substance beakerSubstance = substances.Find(s => s.SubstanceName == mixSubstance.SubstanceName);

            if (beakerSubstance != null)
            {
                // Se la quantità in becher supera il massimo tollerato, il mix non è più possibile
                if (beakerSubstance.Quantity > mixSubstance.Quantity + mixSubstance.Quantity / 3)
                {
                    return false;
                }
            }
        }

        // Controlla se ci sono sostanze nel becher che non esistono nel mix
        foreach (Substance beakerSubstance in substances)
        {
            if (!mix.substances.Any(s => s.SubstanceName == beakerSubstance.SubstanceName))
            {
                return false; 
            }
        }

        return true;
    }

    public float GetLiquidWeight() 
    {
        float liquidWeight = 0;
        if (substances.Count != 0)
        {
            foreach (var substance in substances)
            {
                if (substance.IsSolid == false)
                {
                    liquidWeight += substance.Quantity / 1000;
                }
            }
        }
        return liquidWeight;
    }

    public float GetSolidWeight()
    {
        float solidWeight = 0;
        if (substances.Count != 0)
        {
            foreach (var substance in substances)
            {
                if (substance.IsSolid == true)
                {
                    solidWeight += substance.Quantity / 1000;
                }
            }
        }
        return solidWeight;
    }



}
