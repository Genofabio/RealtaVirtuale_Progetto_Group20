using UnityEngine;

public class ResolutionControllerScript : MonoBehaviour
{
    private Resolution[] resolutions;
    private int currentResolutionIndex;

    void Start()
    {
        // Risoluzioni supportate
        resolutions = Screen.resolutions;

        // Risoluzione attuale
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
                break;
            }
        }

        Debug.Log("Risoluzione attuale: " + resolutions[currentResolutionIndex].width + "x" + resolutions[currentResolutionIndex].height);
    }

    public void RisoluzioneSu()
    {
        if (currentResolutionIndex < resolutions.Length - 1)
        {
            currentResolutionIndex++;
            SetResolution();
        }
    }

    public void RisoluzioneGiù()
    {
        if (currentResolutionIndex > 0)
        {
            currentResolutionIndex--;
            SetResolution();
        }
    }

    private void SetResolution()
    {
        Resolution newResolution = resolutions[currentResolutionIndex];
        Screen.SetResolution(newResolution.width, newResolution.height, Screen.fullScreen);
        Debug.Log("Nuova risoluzione: " + newResolution.width + "x" + newResolution.height);
    }
}
