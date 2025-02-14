using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Assistant : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] Canvas screen;
    [SerializeField] private List<Sprite> images; 
    [SerializeField] private Image displayImage;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> audioList;
    [SerializeField] private AudioClip successClip;
    [SerializeField] private AudioClip failureClip;
    private float delayBetweenAudios = 0.5f;

    [Header("Other")]
    [SerializeField] private GameObject endScreen;

    private int highestStepReached;

    private void Awake()
    {
        audioSource.clip = audioList[0];
        audioSource.Play();
    }
    void Start()
    {
        highestStepReached = -1;

        if (displayImage != null && images.Count > 0)
        {
            displayImage.sprite = images[0];
        }

        if (audioSource != null && audioList.Count > 0)
        {
            audioSource.clip = audioList[0];
        }

        endScreen.SetActive(false);
    }

    public void SetHighestStepReached(int newValue)
    {
        bool isSuccess = newValue > highestStepReached;

        if (isSuccess)
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
        StartCoroutine(PlayStepAudioWithDelay(isSuccess));
    }

    

    private void UpdateImage()
    {
        if (displayImage != null && highestStepReached < images.Count - 1)
        {
            displayImage.sprite = images[highestStepReached + 1]; 
        } else
        {
            endScreen.SetActive(true);
            Debug.Log("Congratulazioni" + highestStepReached);
        }
    }

    public void RepeteStep()
    {
        if (audioSource != null && (highestStepReached + 1) < audioList.Count)
        {
            audioSource.clip = audioList[highestStepReached + 1];
            audioSource.Play();
        }
    }

    private IEnumerator PlayStepAudioWithDelay(bool isSuccess)
    {
        // Riproduce l'audio di successo o fallimento
        if (audioSource != null)
        {
            AudioClip clipToPlay = isSuccess ? successClip : failureClip;
            if (clipToPlay != null)
            {
                audioSource.clip = clipToPlay;
                audioSource.Play();
                yield return new WaitForSeconds(audioSource.clip.length + delayBetweenAudios);
            }
        }

        // Dopo il ritardo, riproduce l'audio dello step corrispondente
        if (audioSource != null && (highestStepReached + 1) < audioList.Count)
        {
            audioSource.clip = audioList[highestStepReached + 1];
            audioSource.Play();
        }
    }
}
