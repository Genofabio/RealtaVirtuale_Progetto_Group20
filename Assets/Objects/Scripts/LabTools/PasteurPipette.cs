using System.Collections.Generic;
using UnityEngine;

using System.Collections.Generic;
using UnityEngine;

public class PasteurPipette : MonoBehaviour
{
    [SerializeField] private float maxVolume;
    private List<Substance> contents = new List<Substance>();

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

    // Preleva liquido da un qualsiasi oggetto che implementa Pourable
    public void Suck(Pourable source, float amountToExtract)
    {
        if (source == null || amountToExtract <= 0 || GetRemainingVolume() <= 0) return;

        amountToExtract = Mathf.Min(amountToExtract, GetRemainingVolume());
        List<Substance> extractedSubstances = source.PickUpVolume(amountToExtract);

        foreach (var sub in extractedSubstances)
        {
            AddSubstance(sub);
        }
    }

    // Versa liquido in un qualsiasi oggetto che implementa Fillable
    public void Drop(Fillable targetContainer, float amountToPour)
    {
        if (targetContainer == null || amountToPour <= 0 || GetCurrentVolume() <= 0) return;

        float targetRemainingVolume = targetContainer.GetRemainingVolume();
        amountToPour = Mathf.Min(amountToPour, GetCurrentVolume(), targetRemainingVolume);

        List<Substance> pouredSubstances = new List<Substance>();
        foreach (var sub in contents)
        {
            float pouredAmount = (sub.Quantity / GetCurrentVolume()) * amountToPour;
            pouredSubstances.Add(new Substance(sub.SubstanceName, sub.SubstanceColor, pouredAmount));
            sub.Quantity -= pouredAmount;
        }

        contents.RemoveAll(sub => sub.Quantity <= 0);
        targetContainer.Fill(pouredSubstances);
    }

    public List<Substance> PickUpVolume(float amountToExtract)
    {
        float totalAmount = GetCurrentVolume();
        if (totalAmount == 0 || amountToExtract <= 0) return new List<Substance>();
        if (amountToExtract > totalAmount) amountToExtract = totalAmount;

        List<Substance> extractedSubstances = new List<Substance>();
        foreach (var sub in contents)
        {
            float extractedAmount = (sub.Quantity / totalAmount) * amountToExtract;
            if (extractedAmount > 0)
            {
                extractedSubstances.Add(new Substance(sub.SubstanceName, sub.SubstanceColor, extractedAmount));
                sub.Quantity -= extractedAmount;
            }
        }

        contents.RemoveAll(sub => sub.Quantity <= 0);
        return extractedSubstances;
    }

    public void Fill(List<Substance> substances)
    {
        foreach (var sub in substances)
        {
            AddSubstance(sub);
        }
    }

    private void AddSubstance(Substance substance)
    {
        if (substance.Quantity <= 0) return;
        Substance existing = contents.Find(s => s.SubstanceName == substance.SubstanceName);
        if (existing != null)
        {
            existing.Quantity += substance.Quantity;
        }
        else
        {
            contents.Add(substance);
        }
    }
}





//public class PasteurPipe : MonoBehaviour, Dropper
//{
//    private bool full;

//    [SerializeField] private float capacity;
//    [SerializeField] private List<Substance> contents;

//    private Liquid liquid;

//    //private void OnValidate()
//    //{
//    //    if (Capacity <= 0)
//    //    {
//    //        Debug.LogWarning("Capacity deve essere maggiore di 0; valore settato a 5ml.");
//    //        Capacity = 5f;
//    //    }
//    //}

//    void Start()
//    {
//        if (capacity <= 0)
//        {
//            Debug.LogWarning("Capacity deve essere maggiore di 0; valore settato a 5ml.");
//            capacity = 5f;
//        }
//        full = false;

//        liquid = GetComponentInChildren<Liquid>();
//        if (liquid == null)
//        {
//            Debug.Log("Liquid NOT found");
//        }
//        else
//        {
//            liquid.SetFillSize(0f);
//        }
//    }

//    public void Drop(Fillable contenitor)
//    {
//        if(full)
//        {

//            //if(contenitor.Fill(Capacity) == 0)
//            //{
//            //    Full = false;
//            //    Debug.Log("Droppato quantità: " + Capacity);
//            //    liquid.SetFillSize(0f);
//            //}
//            //else
//            //{
//            //    Debug.Log("Contenitore pieno, non puoi droppare");
//            //}
//        } else
//        {
//            Debug.Log("Pipetta vuota");
//        }
//    }

//    public void Suck(Pourable contenitor)
//    {
//        if(!full)
//        {
//            //if (contenitor.PickUpVolume(Capacity))
//            //{
//            //    Full = true;
//            //    Debug.Log("Pipetta riempita");
//            //    liquid.SetFillSize(1f);
//            //}
//            //else
//            //{
//            //    Debug.Log("Non c'è abbastanza volume da prendere");
//            //}

//        }
//    }

//    public bool GetFull()
//    {
//        return full;
//    }
//}
