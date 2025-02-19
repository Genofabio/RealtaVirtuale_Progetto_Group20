using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SubstanceMixture
{
    [Header("SubstanceMix Properties")]
    [SerializeField] private bool mixed = false;
    [SerializeField] private bool dried = false;
    [SerializeField] private float dryingTime = 0.0f;
    [SerializeField] private int dryingTemperature = 0;
    [SerializeField] private bool cooled = false;
    [SerializeField] private float coolingTime = 0.0f;

    [Header("Step")]
    [SerializeField] private int experimentStepReached = -1;

    [Header("Colors")]
    [SerializeField] private Color mixtureLiquidColor;
    [SerializeField] private Color mixtureSolidColor;

    [Header("Content")]
    [SerializeField] private List<Substance> substances = new List<Substance>();

    public SubstanceMixture(List<Substance> substances, bool mixed, bool dried, float dryingTime, int dryingTemperature, bool cooled, float coolingTime, int experimentStepReached, Color mixtureLiquidColor, Color mixtureSolidColor)
    {
        Substances = substances.Select(substance => substance.Clone()).ToList();
        Mixed = mixed;
        ExperimentStepReached = experimentStepReached;
        MixtureLiquidColor = mixtureLiquidColor;
        MixtureSolidColor = mixtureSolidColor;
        Dried = dried;
        DryingTime = dryingTime;
        DryingTemperature = dryingTemperature;
        CoolingTime = coolingTime;
        Cooled = cooled;
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

    public bool Dried
    {
        get { return dried; }
        set { dried = value; }
    }

    public float DryingTime
    {
        get { return dryingTime; }
        set { dryingTime = value; }
    }

    public int DryingTemperature
    {
        get { return dryingTemperature; }
        set { dryingTemperature = value; }
    }

    public bool Cooled
    {
        get { return cooled; }
        set { cooled = value; }
    }

    public float CoolingTime
    {
        get { return coolingTime; }
        set { coolingTime = value; }
    }

    public int ExperimentStepReached
    {
        get { return experimentStepReached; }
        set { experimentStepReached = value; }
    }

    public Color MixtureLiquidColor 
    { 
        get { return mixtureLiquidColor; }
        set { mixtureLiquidColor = value; }
    }

    public Color MixtureSolidColor
    {
        get { return mixtureSolidColor; }
        set { mixtureSolidColor = value; }
    }

    public SubstanceMixture Clone()
    {
        return new SubstanceMixture(new List<Substance>(this.Substances), this.Mixed, this.dried, this.dryingTime, this.dryingTemperature, this.cooled, this.coolingTime, this.ExperimentStepReached, this.MixtureLiquidColor, this.MixtureSolidColor);
    }

    public SubstanceMixture GetReference()
    {
        return this;
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
        if (mix.Mixed && ((Mixed && HasSameSubstancePercentageForAll(mix)) || GetCurrentVolume() < 0.001))
        {
            Mixed = true;
        } 
        else
        {
            Mixed = false;
        }

        if (mix.Dried && (Dried || GetCurrentVolume() < 0.001))
        {
            Dried = true;
        }
        else
        {
            Dried = false;
        }

        if (mix.Cooled && (Cooled || GetCurrentVolume() < 0.001))
        {
            Cooled = true;
        }
        else
        {
            Cooled = false;
        }

        float totalVolume = GetCurrentVolume() + mix.GetCurrentVolume();
        Color newMixtureLiquidColor = CalculateMixtureLiquidColor(mix, totalVolume);
        Color newMixtureSolidColor = CalculateMixtureSolidColor(mix, totalVolume);

        foreach (var substance in mix.Substances)
        {
            AddSubstance(substance);
        }

        MixtureLiquidColor = newMixtureLiquidColor;
        MixtureSolidColor = newMixtureSolidColor;
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

    private Color CalculateMixtureLiquidColor(SubstanceMixture mix, float totalVolume)
    {
        if (totalVolume <= 0) return Color.clear;

        float currentVolume = GetCurrentVolume();
        float mixVolume = mix.GetCurrentVolume();

        // Evita divisione per zero
        if (currentVolume + mixVolume <= 0) return Color.clear;

        // Converti i colori in spazio lineare per una miscelazione più realistica
        Color currentColor = MixtureLiquidColor.linear * currentVolume;
        Color addedColor = mix.MixtureLiquidColor.linear * mixVolume;

        // Somma e normalizza in base ai volumi
        Color resultColor = (currentColor + addedColor) / (currentVolume + mixVolume);

        // Riporta in spazio gamma per la visualizzazione corretta
        return resultColor.gamma;
    }

    private Color CalculateMixtureSolidColor(SubstanceMixture mix, float totalVolume)
    {
        if (totalVolume <= 0) return Color.clear;

        float currentVolume = GetCurrentVolume();
        float mixVolume = mix.GetCurrentVolume();

        if (currentVolume + mixVolume <= 0) return Color.clear;

        // Converti in spazio lineare per una miscela più realistica
        Color currentColor = MixtureSolidColor.linear * currentVolume;
        Color addedColor = mix.MixtureSolidColor.linear * mixVolume;

        // Somma e normalizza in base ai volumi
        Color resultColor = (currentColor + addedColor) / (currentVolume + mixVolume);

        // Riporta in spazio gamma per la visualizzazione
        return resultColor.gamma;
    }

    public void StirSubstances()
    {
        mixed = true;
    }

    public List<Substance> ExtractSubstances(float amountToPour, bool extractsOnlyLiquid)
    {
        float totalAmount = GetCurrentVolume();
        float liquidAmount = GetLiquidVolume();
        List<Substance> pouredSubstances = new List<Substance>();

        foreach (var sub in substances)
        {
            if (extractsOnlyLiquid)
            {
                if (sub.IsSolid) continue;

                float pouredAmount = (sub.Quantity / liquidAmount) * amountToPour;
                if (pouredAmount > 0)
                {
                    pouredSubstances.Add(new Substance(sub.SubstanceName, pouredAmount, sub.IsSolid));
                    sub.Quantity -= pouredAmount;
                }
            }
            else
            {
                float pouredAmount = (sub.Quantity / totalAmount) * amountToPour;
                if (pouredAmount > 0)
                {
                    pouredSubstances.Add(new Substance(sub.SubstanceName, pouredAmount, sub.IsSolid));
                    sub.Quantity -= pouredAmount;
                }
            }
        }

        substances.RemoveAll(sub => sub.Quantity <= 0.001);

        if (!(substances.Count > 0))
        {
            Mixed = false;
            Dried = false;
            Cooled = false;
        }

        return pouredSubstances;
    }

    public bool IsSimilarTo(SubstanceMixture mix)
    {
        if (mix.mixed == true && mixed != mix.mixed) return false;
        if (dryingTime < mix.dryingTime - 0.001 || dryingTime > mix.dryingTime + mix.dryingTime / 2 + 0.001) return false;
        if (coolingTime < mix.coolingTime - 0.001) return false;
        if (dryingTemperature != mix.dryingTemperature) return false;

        // Controllo che ogni sostanza in mix sia presente in contents con esattamente la stessa quantità
        foreach (Substance sostanzaMix in mix.substances)
        {
            Substance sostanzaInBecher = substances.Find(s => s.SubstanceName == sostanzaMix.SubstanceName);
            if (sostanzaInBecher == null || sostanzaInBecher.Quantity < sostanzaMix.Quantity - sostanzaMix.Quantity / 20 || sostanzaInBecher.Quantity > sostanzaMix.Quantity + sostanzaMix.Quantity / 20 || sostanzaInBecher.IsSolid != sostanzaMix.IsSolid)
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
        if(!IsQuantityWithinTolerance(mix)) return false;

        if (DryingTemperature > mix.DryingTemperature) return false;

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

    public bool IsQuantityWithinTolerance(SubstanceMixture mix)
    {
        foreach (Substance mixSubstance in mix.substances)
        {
            Substance beakerSubstance = substances.Find(s => s.SubstanceName == mixSubstance.SubstanceName);

            if (beakerSubstance != null)
            {
                // Se la quantità in becher supera il massimo tollerato, il mix non è più possibile
                if (beakerSubstance.Quantity > mixSubstance.Quantity + mixSubstance.Quantity / 20)
                {
                    return false;
                }
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
                    liquidWeight += substance.Quantity;
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
                    solidWeight += substance.Quantity;
                }
            }
        }
        return solidWeight;
    }

    public void Dry(float deltaTime, int temperature)
    {
        dryingTime += deltaTime;

        if (dryingTemperature != temperature)
        {
            dryingTime = 0f;
        }

        dryingTemperature = temperature;
    }

    public void CoolDown(float deltaTime)
    {
        coolingTime += deltaTime;
    }

}
