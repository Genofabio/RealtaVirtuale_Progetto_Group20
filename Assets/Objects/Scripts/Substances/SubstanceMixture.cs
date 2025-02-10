using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SubstanceMixture
{
    [SerializeField] private List<Substance> substances = new List<Substance>();

    private bool mixed = false;
    private int experimentStepReached = -1;

    public SubstanceMixture(List<Substance> substances, bool mixed, int experimentStepReached)
    {
        Substances = substances;
        Mixed = mixed;
        ExperimentStepReached = experimentStepReached;
    }

    public List<Substance> Substances
    {
        get { return substances; }
        set { substances = value; }
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
        foreach (var substance in mix.Substances)
        {
            AddSubstance(substance);
        }
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

        substances.RemoveAll(sub => sub.Quantity <= 0);

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
