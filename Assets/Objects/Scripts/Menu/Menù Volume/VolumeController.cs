using UnityEngine;

public class VolumeController : MonoBehaviour
{
    public float volumeStep = 0.1f; // Su/giù volume

    void Start()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("GameVolume", 1.0f);
        //Debug.Log("Volume iniziale: " + AudioListener.volume);
    }

    public void VolumeSu()
    {
        AudioListener.volume = Mathf.Clamp(AudioListener.volume + volumeStep, 0f, 1f);
        PlayerPrefs.SetFloat("GameVolume", AudioListener.volume);
        //Debug.Log("Volume aumentato: " + AudioListener.volume);
    }

    public void VolumeGiù()
    {
        AudioListener.volume = Mathf.Clamp(AudioListener.volume - volumeStep, 0f, 1f);
        PlayerPrefs.SetFloat("GameVolume", AudioListener.volume);
        //Debug.Log("Volume diminuito: " + AudioListener.volume);
    }
}