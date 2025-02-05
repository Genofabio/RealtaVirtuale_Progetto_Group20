using UnityEngine;

public class PrecisionBalance : MonoBehaviour
{
    private float weight = 0f;
    private GameObject toWeightObject;
    
    public void AddWeight(float w)
    {
        weight += w;
        Debug.Log("Oggetto poggiato sulla bilancia, peso totale: " + weight);
    }

    public void RemoveWeight(float w)
    {
        weight -= w;
        Debug.Log("Oggetto rimosso dalla bilancia, peso totale: " + weight);
    }

    public void Tare()
    {
        weight = 0f;
        Debug.Log("Taratura bilancia effettuata, peso totale: " + weight);
    }
}
