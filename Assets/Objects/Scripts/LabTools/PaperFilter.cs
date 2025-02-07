using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;


using System.Linq; // Necessario per LINQ

public class PaperFilter : MonoBehaviour, Filter
{
    private Becher becher;
    [SerializeField] private List<Substance> filtrableSubstances;
    [SerializeField] private float toFilterVolume;
    [SerializeField] private float filteredVolume = 0;

    void Awake()
    {
        filtrableSubstances = new List<Substance>();
        //filtrableSubstances.Add(new Substance("H2O", 0));
        //filtrableSubstances.Add(new Substance("HCl", 0));
        //toFilterVolume = 10f;
    }

    public void FilterLiquid(List<Substance> toFilterSubstances)
    {
        // Estrai i nomi delle sostanze da entrambe le liste
        var toFilterNames = toFilterSubstances.Select(s => s.SubstanceName).OrderBy(n => n);
        var filtrableNames = filtrableSubstances.Select(s => s.SubstanceName).OrderBy(n => n);

        // Confronta le liste dei nomi (in ordine per ignorare la disposizione)
        if (toFilterNames.SequenceEqual(filtrableNames))
        {
            float actualFiltrableVolume = toFilterSubstances.Sum(s => s.Quantity);
            filteredVolume += actualFiltrableVolume;
            if (filteredVolume >= toFilterVolume)
            {
                //filteredVolume = toFilterVolume;
                //Debug.Log("Filtraggio completato, filteredVolume: " + filteredVolume);
            }
            else
            { 
                //Debug.Log("Filtrata quantità: " + actualFiltrableVolume + ", mancano: " + (toFilterVolume - filteredVolume));
            }
        }
        //else
        //{
        //    Debug.Log("Le sostanze non corrispondono esattamente a quelle filtrabili");
        //}
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
        //rimuovere il filtro graficamente
    }

    //public bool IsFilterOn()
    //{
    //    return becher != null;
    //}
}
