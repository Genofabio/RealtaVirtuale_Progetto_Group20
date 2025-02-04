using UnityEngine;

public class Becher : MonoBehaviour, Fillable, Pourable
{
    public int maxVolume;
    public int currentVolume;

    public int Fill(int volume)
    {
        if (currentVolume == maxVolume)
        {
            //Debug.Log("NON ENTRAAAAAAAAAAA");
            return volume;
        }
        else if (currentVolume + volume <= maxVolume)
        {
            currentVolume += volume;
            //Debug.Log("ci entrava tutto");
            return 0;

        }
        else
        {
            int remainingVolume = maxVolume - currentVolume;
            currentVolume = maxVolume;
            //Debug.Log("Va di fori: " + remainingVolume);
            return remainingVolume;
        }
    }

    public void Pour(Fillable contenitor)
    {
        if (currentVolume > 0)
        {
            currentVolume = contenitor.Fill(currentVolume);
        }
    }   
}
