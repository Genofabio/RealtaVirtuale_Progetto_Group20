using UnityEngine;

public class PasteurPipe : MonoBehaviour, Dropper
{
    bool Full;
    [SerializeField] private float Capacity;
    Liquid liquid;

    private void OnValidate()
    {
        if (Capacity <= 0)
        {
            Debug.LogWarning("Capacity deve essere maggiore di 0; valore settato a 5ml.");
            Capacity = 5f;
        }
    }

    void Start()
    {
        if(Capacity <= 0)
        {
            Debug.LogWarning("Capacity deve essere maggiore di 0; valore settato a 5ml.");
            Capacity = 5f;
        }
        Full = false;

        liquid = GetComponentInChildren<Liquid>();
        if (liquid == null)
        {
            Debug.Log("Liquid NOT found");
        }
        else
        {
            liquid.SetFillSize(0f);
        }
    }

    public void Drop(Fillable contenitor)
    {
        if(Full)
        {
            if(contenitor.Fill(Capacity) == 0)
            {
                Full = false;
                Debug.Log("Droppato quantità: " + Capacity);
                liquid.SetFillSize(0f);
            }
            else
            {
                Debug.Log("Contenitore pieno, non puoi droppare");
            }
        } else
        {
            Debug.Log("Pipetta vuota");
        }
    }

    public void PickUp(Pourable contenitor)
    {
        if(!Full)
        {
            if (contenitor.PickUpVolume(Capacity))
            {
                Full = true;
                Debug.Log("Pipetta riempita");
                liquid.SetFillSize(1f);
            }
            else
            {
                Debug.Log("Non c'è abbastanza volume da prendere");
            }

        }
    }

    public bool GetFull()
    {
        return Full;
    }
}
