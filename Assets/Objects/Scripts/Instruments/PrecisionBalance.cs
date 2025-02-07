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
        Debug.Log("Peso totale: " + RoundToSignificantFigures(totalWeight + tare, 4));
    }

    public void Tare()
    {
        tare = -totalWeight;
        Debug.Log("Taratura bilancia effettuata, peso totale: " + RoundToSignificantFigures(totalWeight + tare, 4));
    }

    private float RoundToSignificantFigures(float value, int sigFigures)
    {
        if (value == 0) return 0;

        float scale = Mathf.Pow(10, sigFigures - Mathf.FloorToInt(Mathf.Log10(Mathf.Abs(value))) - 1);
        return Mathf.Round(value * scale) / scale;
    }
}
