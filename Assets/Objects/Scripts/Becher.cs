using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Becher : MonoBehaviour, Fillable, Pourable
{
    Liquid liquid;
    [SerializeField] private float maxVolume;
    public float currentVolume;

    private void OnValidate()
    {
        if (maxVolume <= 0)
        {
            Debug.LogWarning("Value deve essere maggiore di 0. Impostazione modificata a 20ml.");
            maxVolume = 20f; 
        }
    }

    void Start()
    {
        liquid = GetComponentInChildren<Liquid>();
        if (liquid == null)
        {
            Debug.Log("Liquid NOT found");
        }
        else
        {
            liquid.SetFillSize(currentVolume / maxVolume);
        }
    }
    public float Fill(float volume)
    {
        if (currentVolume == maxVolume)
        {
            Debug.Log("NON ENTRAAAAAAAAAAA");
            return volume;
        }
        else if (currentVolume + volume <= maxVolume)
        {
            currentVolume += volume;
            liquid.SetFillSize(currentVolume / maxVolume);
            //Debug.Log("ci entrava tutto");
            return 0;

        }
        else
        {
            float remainingVolume = maxVolume - currentVolume;
            currentVolume = maxVolume;
            liquid.SetFillSize(currentVolume / maxVolume);  
            //Debug.Log("Va di fori: " + remainingVolume);
            return remainingVolume;
        }
    }

    public void Pour(Fillable contenitor)
    {
        if (currentVolume > 0)
        {
            currentVolume = contenitor.Fill(currentVolume);
            liquid.SetFillSize(currentVolume/maxVolume);
        }
    }   
}
