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
       objects.Add(obj);
       UpdateWeightText();
    }

    public void RemoveWeight(Rigidbody obj)
    {
        objects.Remove(obj);
        UpdateWeightText();
    }

    public void WeightObjects()
    {
        totalWeight = 0f;
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
        totalWeight = 0f;
        foreach (var obj in objects)
        {
            totalWeight += obj.mass;
        }
        return totalWeight;
    }

    private float RoundToDecimalPlaces(float value, int decimalPlaces)
    {
        float factor = Mathf.Pow(10, decimalPlaces);
        return Mathf.Round(value * factor) / factor;
    }

    private void UpdateWeightText()
    {
        if (weightText != null)
        {
            Debug.Log("Entrato");
            Debug.Log(RoundToDecimalPlaces(totalWeight + currentTare, 2));
            weightText.text = RoundToDecimalPlaces(totalWeight + currentTare, 2).ToString();
        }
    }

}
