using UnityEngine;

[System.Serializable]
public class Substance
{
    [SerializeField] private string substanceName;
    [SerializeField] private float quantity;
    //[SerializeField] private Color color;

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
        return new Substance(this.substanceName, this.quantity);
    }

    public Substance(string name, float qty)
    {
        SubstanceName = name;
        //SubstanceColor = col;
        Quantity = qty;
    }
}
