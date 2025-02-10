using UnityEngine;

[System.Serializable]
public class Substance
{
    [SerializeField] private string substanceName;
    [SerializeField] private float quantity;
    //[SerializeField] private Color color;
    [SerializeField] private bool isSolid;

    public Substance(string name, float qty, bool solid)
    {
        SubstanceName = name;
        Quantity = qty;
        isSolid = solid;
    }

    public string SubstanceName
    {
        get { return substanceName; }
        set { substanceName = value; }
    }

    public float Quantity
    {
        get { return quantity; }
        set { quantity = Mathf.Max(0, value); }
    }

    public Substance Clone()
    {
        return new Substance(this.substanceName, this.quantity, this.IsSolid);
    }

    public bool IsSolid
    {
        get { return isSolid; }
        set { isSolid = value; }
    }
}
