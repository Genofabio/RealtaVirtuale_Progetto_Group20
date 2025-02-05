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
        becher.SetFilterOn(this);
        this.becher = becher;
        //posizionare il filtro sopra il becher graficamente
    }

    public bool IsFilterOn()
    {
        return becher != null;
    }

    public void RemoveFilter()
    {
        Debug.Log("Rimuovo il filtro...");
        becher.SetFilterOff();
        becher = null;
        //rimuovere il filtro graficamente
    }
}
