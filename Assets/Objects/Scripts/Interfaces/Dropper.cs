using UnityEngine;

public interface Dropper
{
    public void Drop(Fillable contenitor);
    public void Suck(Pourable contenitor);
    public bool IsFull();
}
