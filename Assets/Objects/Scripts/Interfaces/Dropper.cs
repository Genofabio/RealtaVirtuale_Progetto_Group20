using UnityEngine;

public interface Dropper
{
    public void Drop(Fillable contenitor);
    public void PickUp(Pourable contenitor);
    public bool GetFull();
}
