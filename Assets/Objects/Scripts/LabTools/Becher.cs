using UnityEngine;
using static UnityEditor.Experimental.GraphView.Port;
using static UnityEngine.Rendering.DebugUI;

public class Becher : MonoBehaviour, Fillable, Pourable
{
    Liquid liquid;
    [SerializeField] private float maxVolume;
    public float currentVolume;
    private bool isFilterOn = false;
    private Filter filter = null;
    [SerializeField] private Transform filterPosition;

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
            if(isFilterOn)
            {
                filter.FilterLiquid();
            }
            currentVolume += volume;
            liquid.SetFillSize(currentVolume / maxVolume);
            //Debug.Log("ci entrava tutto");
            return 0;

        }
        else
        {
            if(isFilterOn)
            {
                Debug.Log("Non c'è spazio a sufficienza per filtrare tutto il liquido");
            }
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
