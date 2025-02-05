using UnityEngine;
using static UnityEditor.Experimental.GraphView.Port;

public class SubstanceVial : MonoBehaviour, Pourable
{
    public float currentVolume;
    public float maxVolume;
    Liquid liquid;

    private void OnValidate()
    {
        if (maxVolume <= 0)
        {
            Debug.LogWarning("maxVolume deve essere maggiore di 0. Fialetta piena di 10ml.");
            maxVolume = 10f;
            currentVolume = 10f;
        }
    }

    void Start()
    {
        if (maxVolume <= 0)
        {
            Debug.LogWarning("maxVolume deve essere maggiore di 0. Fialetta piena di 10ml.");
            maxVolume = 10f;
            currentVolume = 10f;
        }

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

    public bool PickUpVolume(float volume)
    {
        if (currentVolume >= volume)
        {
            currentVolume -= volume;
            liquid.SetFillSize(currentVolume / maxVolume);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Pour(Fillable contenitor)
    {
        if (currentVolume > 0)
        {
            currentVolume = contenitor.Fill(currentVolume);
            liquid.SetFillSize(currentVolume / maxVolume);
        }
    }


}
