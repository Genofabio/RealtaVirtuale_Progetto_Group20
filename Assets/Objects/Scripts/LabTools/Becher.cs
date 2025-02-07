using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.Port;
using static UnityEngine.Rendering.DebugUI;

public class Becher : MonoBehaviour, Fillable, Pourable
{
    [SerializeField] private List<Substance> contents;
    [SerializeField] private float maxVolume;

    private LiquidRenderer liquid;

    private bool isFilterOn = false;
    private Filter filter = null;

    private float becherMass;

    public List<Substance> Contents => contents;

    private void OnValidate()
    {
        if (maxVolume <= 0)
        {
            Debug.LogWarning("maxVolume deve essere maggiore di 0. Impostazione modificata a 20ml.");
            maxVolume = 20f; 
        }
    }

    void Start()
    {
        if (maxVolume <= 0)
        {
            Debug.LogWarning("Capacity deve essere maggiore di 0. Impostazione modificata a 20ml.");
            maxVolume = 20f;
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

        TryGetComponent<Rigidbody>(out var rb);
        becherMass = rb.mass;

        RefreshTotalWeight();
    }

    public float GetCurrentVolume()
    {
        float sum = 0f;
        foreach (var substance in contents)
        {
            sum += substance.Quantity;
        }
        return sum;
    }

    public float GetRemainingVolume()
    {
        return maxVolume - GetCurrentVolume();
    }

    public void AddSubstance(Substance substance)
    {
        if (substance.Quantity <= 0) return;
        float totalAmount = GetCurrentVolume() + substance.Quantity;

        //if (totalAmount > maxVolume) substance.Quantity -= (totalAmount - maxVolume);

        Substance existing = contents.Find(s => s.SubstanceName == substance.SubstanceName);
        if (existing != null)
        {
            existing.Quantity += substance.Quantity;
        }
        else
        {
            contents.Add(substance);
        }

        liquid.SetFillSize(totalAmount / maxVolume);
    }

    public void RefreshTotalWeight()
    {
        TryGetComponent<Rigidbody>(out var rb);
        float liquidWeight = 0f;
        if (contents.Count != 0)
        {
            foreach (var substance in contents)
            {
                liquidWeight += substance.Quantity / 1000;
            }
        }
        rb.mass = becherMass + liquidWeight;
    }

    public void Fill(List<Substance> substances)
    {
        if(isFilterOn)
        {
            filter.FilterLiquid(substances);
        }
        foreach (var substance in substances)
        {
            AddSubstance(substance);

            TryGetComponent<Rigidbody>(out var rb);
            rb.mass += substance.Quantity/1000;
        }
    }

    //public float Fill(float volume)
    //{
    //    if (currentVolume == maxVolume)
    //    {
    //        Debug.Log("NON ENTRAAAAAAAAAAA");
    //        return volume;
    //    }
    //    else if (currentVolume + volume <= maxVolume)
    //    {
    //        if(isFilterOn)
    //        {
    //            filter.FilterLiquid();
    //        }
    //        currentVolume += volume;
    //        liquid.SetFillSize(currentVolume / maxVolume);
    //        //Debug.Log("ci entrava tutto");
    //        return 0;

    //    }
    //    else
    //    {
    //        if(isFilterOn)
    //        {
    //            Debug.Log("Non c'è spazio a sufficienza per filtrare tutto il liquido");
    //        }
    //        float remainingVolume = maxVolume - currentVolume;
    //        currentVolume = maxVolume;
    //        liquid.SetFillSize(currentVolume / maxVolume);  
    //        //Debug.Log("Va di fori: " + remainingVolume);
    //        return remainingVolume;
    //    }
    //}

    public void Pour(Fillable targetContainer, float amountToPour)
    {
        float totalAmount = GetCurrentVolume();
        float targetRemainingVolume = targetContainer.GetRemainingVolume();
        if (totalAmount <= 0 || amountToPour <= 0 || targetRemainingVolume <= 0) return;
        if (amountToPour > totalAmount) amountToPour = totalAmount;
        if (amountToPour > targetRemainingVolume) amountToPour = targetRemainingVolume;

        // Calcola la percentuale da trasferire
        List<Substance> pouredSubstances = new List<Substance>();
        foreach (var sub in contents)
        {
            float pouredAmount = (sub.Quantity / totalAmount) * amountToPour;
            if (pouredAmount > 0)
            {
                pouredSubstances.Add(new Substance(sub.SubstanceName, pouredAmount));
                sub.Quantity -= pouredAmount;

                RefreshTotalWeight();
            }
        }

        // Rimuove le sostanze con quantità zero
        contents.RemoveAll(sub => sub.Quantity <= 0);

        liquid.SetFillSize(GetCurrentVolume() / maxVolume);

        // Versa nel becher di destinazione
        targetContainer.Fill(pouredSubstances);
    }

    public List<Substance> PickUpVolume(float amountToExtract)
    {
        float totalAmount = GetCurrentVolume();
        if (totalAmount == 0 || amountToExtract <= 0) return new List<Substance>();
        if (amountToExtract > totalAmount) amountToExtract = totalAmount;

        // Calcola la percentuale da estrarre
        List<Substance> extractedSubstances = new List<Substance>();
        foreach (var sub in contents)
        {
            float extractedAmount = (sub.Quantity / totalAmount) * amountToExtract;
            if (extractedAmount > 0)
            {
                extractedSubstances.Add(new Substance(sub.SubstanceName, extractedAmount));
                sub.Quantity -= extractedAmount;

                RefreshTotalWeight();
            }
        }

        // Rimuove le sostanze con quantità zero
        contents.RemoveAll(sub => sub.Quantity <= 0);
        liquid.SetFillSize(GetCurrentVolume() / maxVolume);

        return extractedSubstances;
        //if (currentVolume >= volume)
        //{
        //    currentVolume -= volume;
        //    liquid.SetFillSize(currentVolume / maxVolume);
        //    return true;
        //}
        //else
        //{
        //    return false;
        //}
    }

    public void SetFilterOn(Filter filter)
    {
        if(isFilterOn == false) { 
            this.filter = filter;
            isFilterOn = true;
            Debug.Log("Filtro applicato");
        } else
        {
            Debug.Log(isFilterOn);
            Debug.Log("C'è già un filtro sul becher"); 
        }
    }

    public void SetFilterOff()
    {
        if(isFilterOn == true /*&& filter != null*/)
        {
            filter = null;
            isFilterOn = false;

        } else
        {
            Debug.Log("Non c'è nessun filtro"); //tecnicamente non deve mai comparirre sto messaggio
        }
    }
}
