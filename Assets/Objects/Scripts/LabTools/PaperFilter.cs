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

    void Awake()
    {
        filteredSubstances = new SubstanceMixture(new List<Substance>(), false, false, 0, false, 0, - 1, Color.clear, Color.clear);
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

        filteredSubstances.AddSubstanceMixture(new SubstanceMixture(solids, mix.Mixed, mix.Dried, mix.DryingTime, mix.Cooled, mix.CoolingTime, mix.ExperimentStepReached, mix.MixtureLiquidColor, mix.MixtureSolidColor));
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

        SubstanceMixture pouredMix = new SubstanceMixture(new List<Substance>(), filteredSubstances.Mixed, filteredSubstances.Dried, filteredSubstances.DryingTime, filteredSubstances.Cooled, filteredSubstances.CoolingTime, filteredSubstances.ExperimentStepReached, filteredSubstances.MixtureLiquidColor, filteredSubstances.MixtureSolidColor);
        pouredMix.Substances = filteredSubstances.ExtractSubstances(amountToPour);

        solidRenderer.SetFillSize(GetCurrentVolume() / maxCapacity);
        //RefreshTotalWeight();

        targetContainer.Fill(pouredMix);

        //experimentManager.CheckAndModifyStep(containedMixture);
    }

    public SubstanceMixture PickUpVolume(float amount)
    {
        throw new System.NotImplementedException();
    }

    public float GetCurrentVolume()
    {
        return filteredSubstances.GetCurrentVolume();
    }
}
