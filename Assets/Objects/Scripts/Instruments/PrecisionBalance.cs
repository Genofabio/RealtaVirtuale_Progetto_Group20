using System.Collections.Generic;
using UnityEngine;


public class PrecisionBalance : MonoBehaviour
{
    private float totalWeight = 0f;
    private float tare = 0f;
    private List<Rigidbody> objects = new List<Rigidbody>();

    public void AddWeight(Rigidbody obj)
    {
       objects.Add(obj);
    }

    public void RemoveWeight(Rigidbody obj)
    {
        objects.Remove(obj);
    }

    public void WeightObjects()
    {
        totalWeight = 0f;
        foreach (var obj in objects)
        {
            totalWeight += obj.mass;
        }
        Debug.Log("Peso totale: " + RoundToDecimalPlaces(totalWeight + tare, 4));
    }

    public void Tare()
    {
        tare = -totalWeight;
        Debug.Log("Taratura bilancia effettuata, peso totale: " + RoundToDecimalPlaces(totalWeight + tare, 4));
    }

    private float RoundToDecimalPlaces(float value, int decimalPlaces)
    {
        float factor = Mathf.Pow(10, decimalPlaces);
        return Mathf.Round(value * factor) / factor;
    }

}
