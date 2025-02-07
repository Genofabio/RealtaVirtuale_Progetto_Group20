using UnityEngine;

public class PrecisionBalance : MonoBehaviour
{
    private float totalWeight = 0f;
    private GameObject toWeightObject;
    
    public void AddWeight(float w)
    {
        totalWeight += w;
        Debug.Log("Oggetto poggiato sulla bilancia, peso totale: " + totalWeight);
    }

    public void RemoveWeight(float w)
    {
        totalWeight -= w;
        Debug.Log("Oggetto rimosso dalla bilancia, peso totale: " + totalWeight);
    }

    public void Tare()
    {
        totalWeight = 0f;
        Debug.Log("Taratura bilancia effettuata, peso totale: " + totalWeight);
    }
}
