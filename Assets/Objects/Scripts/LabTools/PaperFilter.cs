using UnityEngine;

public class PaperFilter : MonoBehaviour, Filter
{
    private Becher becher;
    public void FilterLiquid()
    {
        Debug.Log("Filtraggio effettuato, aggiungere rappresentazione grafica");
    }

    public void ApplyFilter(Becher becher)
    {
        //becher.SetFilterOn(this);
        this.becher = becher;

        Rigidbody rb = GetComponent<Rigidbody>();
        Destroy(rb);

        //transform.position = becher.getFilterPosition().position;
        //transform.SetParent(becher.getFilterPosition());
    }

    void Update()
    {
        if (/*isAttached && */becher != null)
        {
            //transform.position = becher.getFilterPosition().position;
        }
    }

    public bool IsFilterOn()
    {
        return becher != null;
    }

    public void RemoveFilter()
    {
        Debug.Log("Rimuovo il filtro...");
        //becher.SetFilterOff();
        becher = null;
        //rimuovere il filtro graficamente
    }
}
