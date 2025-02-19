using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

using System.Linq; // Necessario per LINQ

public class PaperFilter : MonoBehaviour, Filter, Pourable
{
    private Becher becher;
    [SerializeField] private SubstanceMixture filteredSubstances;
    [SerializeField] private float maxCapacity;
    [SerializeField] private float filteredVolume = 0;
    private SolidRenderer solidRenderer;

    private ExperimentManager experimentManager;

    private Rigidbody rb;

    void Awake()
    {
        filteredSubstances = new SubstanceMixture(new List<Substance>(), false, false, 0, 0, false, 0, - 1, Color.clear, Color.clear);
    }

    void Start()
    {
        experimentManager = FindFirstObjectByType<ExperimentManager>();

        if (maxCapacity <= 0)
        {
            Debug.LogWarning("maxCapacity deve essere maggiore di 0. Filtro pieno di 10ml.");
            maxCapacity = 10f;
        }

        solidRenderer = GetComponentInChildren<SolidRenderer>();
        if (solidRenderer == null)
        {
            Debug.LogWarning("Solid NOT found");
        }
        else
        {
            solidRenderer.SetFillSize(0); // dando per scontato che non tu possa inizializzare un filtro con della roba dentro
        }

        rb = GetComponent<Rigidbody>();
    }

    public SubstanceMixture FilterLiquid(SubstanceMixture mix)
    {
        if (filteredVolume >= maxCapacity)
        {
            Debug.LogWarning("Il filtro è pieno");
            return mix;
        }

        List<Substance> solids = mix.Substances.Where(s => s.IsSolid).ToList();
        foreach (var solid in solids)
        {
            filteredVolume += solid.Quantity;
        }

        if (filteredSubstances.ExperimentStepReached < mix.ExperimentStepReached)
        {
            experimentManager.SetMixtureStepAndUpdateCount(filteredSubstances, mix.ExperimentStepReached);
        }


        SubstanceMixture filteredMixture = new SubstanceMixture(solids, mix.Mixed, mix.Dried, mix.DryingTime, mix.DryingTemperature, mix.Cooled, mix.CoolingTime, mix.ExperimentStepReached, mix.MixtureLiquidColor, mix.MixtureSolidColor);
        filteredSubstances.AddSubstanceMixture(filteredMixture);
        rb.mass += filteredMixture.GetSolidWeight();

        solidRenderer.SetFillSize(filteredVolume / maxCapacity);
        solidRenderer.SetColor(mix.MixtureSolidColor);

        if (filteredSubstances.ExperimentStepReached < 0)
        {
            filteredSubstances.ExperimentStepReached = mix.ExperimentStepReached;
        }

        experimentManager.CheckAndModifyStep(filteredSubstances);

        // Crea una nuova lista senza i solidi
        mix.Substances = mix.Substances.Where(s => !s.IsSolid).ToList();

        return mix;
    }

    public void ApplyFilter(Becher becher)
    {
        Debug.Log("Applico il filtro...");
        becher.SetFilterOn(this);
        this.becher = becher;
    }

    public void RemoveFilter()
    {
        Debug.Log("Rimuovo il filtro...");
        becher.SetFilterOff();
        becher = null;
    }

    public void Pour(Fillable targetContainer, float amountToPour)
    {
        float totalAmount = GetCurrentVolume();
        float targetRemainingVolume = targetContainer.GetRemainingVolume();
        if (totalAmount <= 0 || amountToPour <= 0 || targetRemainingVolume <= 0) return;
        if (amountToPour > totalAmount) amountToPour = totalAmount;
        if (amountToPour > targetRemainingVolume) amountToPour = targetRemainingVolume;

        SubstanceMixture pouredMix = new SubstanceMixture(new List<Substance>(), filteredSubstances.Mixed, filteredSubstances.Dried, filteredSubstances.DryingTime, filteredSubstances.DryingTemperature, filteredSubstances.Cooled, filteredSubstances.CoolingTime, filteredSubstances.ExperimentStepReached, filteredSubstances.MixtureLiquidColor, filteredSubstances.MixtureSolidColor);
        pouredMix.Substances = filteredSubstances.ExtractSubstances(amountToPour, false);

        rb.mass -= pouredMix.GetSolidWeight();

        solidRenderer.SetFillSize(GetCurrentVolume() / maxCapacity);
        //RefreshTotalWeight();

        targetContainer.Fill(pouredMix);

        experimentManager.CheckAndModifyStep(filteredSubstances);
    }

    public SubstanceMixture PickUpVolume(float amountToExtract, bool picksUpOnlyLiquid) 
    {
        if(picksUpOnlyLiquid) return new SubstanceMixture(new List<Substance>(), filteredSubstances.Mixed, filteredSubstances.Dried, filteredSubstances.DryingTime, filteredSubstances.DryingTemperature, filteredSubstances.Cooled, filteredSubstances.CoolingTime, filteredSubstances.ExperimentStepReached, filteredSubstances.MixtureLiquidColor, filteredSubstances.MixtureSolidColor);
        else
        {
            SubstanceMixture extractedMix = new SubstanceMixture(new List<Substance>(), filteredSubstances.Mixed, filteredSubstances.Dried, filteredSubstances.DryingTime, filteredSubstances.DryingTemperature, filteredSubstances.Cooled, filteredSubstances.CoolingTime, filteredSubstances.ExperimentStepReached, filteredSubstances.MixtureLiquidColor, filteredSubstances.MixtureSolidColor);
            if (filteredVolume == 0 || amountToExtract <= 0) return extractedMix;
            if (amountToExtract > filteredVolume) amountToExtract = filteredVolume;

            List<Substance> extractedSubstances = filteredSubstances.ExtractSubstances(amountToExtract, false);
            extractedMix.Substances = extractedSubstances;

            UpdateSubstanceRenderFill();
            //RefreshTotalWeight();

            return extractedMix;
        }
    }

    public void UpdateSubstanceRenderFill()
    {
        if (GetCurrentVolume() > 0.01)
        {
            solidRenderer.SetFillSize(GetCurrentVolume() / maxCapacity);
        }
    }

    public float GetCurrentVolume()
    {
        return filteredSubstances.GetCurrentVolume();
    }
}
