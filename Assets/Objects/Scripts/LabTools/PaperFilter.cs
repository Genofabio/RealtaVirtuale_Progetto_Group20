using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;


using System.Linq; // Necessario per LINQ

public class PaperFilter : MonoBehaviour, Filter
{
    private Becher becher;
    [SerializeField] private SubstanceMixture filteredSubstances;
    [SerializeField] private float maxCapacity;
    [SerializeField] private float filteredVolume = 0;
    private SolidRenderer solidRenderer;

    void Awake()
    {
        filteredSubstances = new SubstanceMixture(new List<Substance>(), false, -1);
    }

    void Start()
    {
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
        filteredSubstances.AddSubstanceMixture(new SubstanceMixture(solids, mix.Mixed, mix.ExperimentStepReached));
        solidRenderer.SetFillSize(filteredVolume / maxCapacity);

        mix.Substances.RemoveAll(s => s.IsSolid);

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
}
