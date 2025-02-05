using UnityEngine;

public interface Pourable
{
    public void Pour(Fillable contenitor);

    public bool PickUpVolume(float volume);
}
