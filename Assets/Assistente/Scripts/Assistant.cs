using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Assistant : MonoBehaviour
{
    [SerializeField] Canvas screen;
    [SerializeField] private List<Sprite> images; 
    [SerializeField] private Image displayImage; 

    [SerializeField] private int highestStepReached;

    void Start()
    {
        highestStepReached = -1;

        if (displayImage != null && images.Count > 0)
        {
            displayImage.sprite = images[0];
        }
    }

    public void SetHighestStepReached(int newValue)
    {
        if (highestStepReached < newValue)
        {
            highestStepReached = newValue;
            Debug.Log("Complimenti hai raggiunto lo step: " + highestStepReached);
        } 
        else
        {
            highestStepReached = newValue;
            Debug.Log("Oh noooooooo hai raggiunto lo step: " + highestStepReached);
        }

        UpdateImage();
    }

    private void UpdateImage()
    {
        if (displayImage != null && highestStepReached < images.Count)
        {
            displayImage.sprite = images[highestStepReached + 1]; 
        }
    }
}
