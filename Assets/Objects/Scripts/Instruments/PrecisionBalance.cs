using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class PrecisionBalance : MonoBehaviour
{
    private float totalWeight = 0f;
    private List<Rigidbody> objects = new List<Rigidbody>();

    private float currentTare = 0f;

    public TextMeshProUGUI weightText;

    public void AddWeight(Rigidbody obj)
    {
        if (!objects.Contains(obj))
        {
            objects.Add(obj);
            WeightObjects();
        }
    }

    public void RemoveWeight(Rigidbody obj)
    {
        if (objects.Contains(obj))
        {
            objects.Remove(obj);
            WeightObjects();
        }
    }

    public void WeightObjects()
    {
        totalWeight = 0f;

        objects.RemoveAll(obj => obj == null);

        foreach (var obj in objects)
        {
            totalWeight += obj.mass;
        }
        UpdateWeightText();
    }

    public void SetCurrentTare(float value)
    {
        currentTare = value;
        UpdateWeightText();
    }

    public float GetTotalWeight()
    {
        WeightObjects();
        return totalWeight;
    }

    private float RoundToDecimalPlaces(float value, int decimalPlaces)
    {
        float factor = Mathf.Pow(10f, decimalPlaces);
        return Mathf.Round(value * factor) / factor;
    }

    private void UpdateWeightText()
    {
        if (weightText != null)
        {
            weightText.text = RoundToDecimalPlaces(totalWeight + currentTare, 4).ToString();
        }
    }

}
