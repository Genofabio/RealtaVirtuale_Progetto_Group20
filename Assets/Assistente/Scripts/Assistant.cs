using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Assistant : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private Image displayImage;
    [SerializeField] private Image success;
    [SerializeField] private Image failure;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip successClip;
    [SerializeField] private AudioClip failureClip;
    private float delayBetweenAudios = 0.5f;

    [Header("Other")]
    private MovementController movementController;

    private int highestStepReached;

    [Header("Timer screen")]
    [SerializeField] private GameObject timerScreen;
    [SerializeField] private TextMeshProUGUI timeText;
    private Coroutine hideTimerCoroutine;
    private float hideDelay = 0.5f; // Tempo di attesa prima di nascondere il timer

    private void Awake()
    {
        //audioSource.clip = audioList[0];
        //audioSource.Play();
        movementController = GetComponent<MovementController>();
    }
    void Start()
    {
        highestStepReached = -1;

        //if (displayImage != null && images.Count > 0)
        //{
        //    displayImage.sprite = images[0];
        //}

        //if (audioSource != null && audioList.Count > 0)
        //{
        //    audioSource.clip = audioList[0];
        //}
        timerScreen.SetActive(false);
    }

    public void SetHighestStepReached(int newValue)
    {
        bool isSuccess = newValue > highestStepReached;

        if (isSuccess)
        {
            highestStepReached = newValue;
            Debug.Log("Complimenti hai raggiunto lo step: " + highestStepReached);
            StartCoroutine(ShowImageForDuration(success));
        }
        else
        {
            highestStepReached = newValue;
            Debug.Log("Oh noooooooo hai raggiunto lo step: " + highestStepReached);
            StartCoroutine(ShowImageForDuration(failure));
        }

        UpdatePath();
        //UpdateImage();
        StartCoroutine(PlayStepAudioWithDelay(isSuccess));
    }

    // Coroutine per mostrare l'immagine per 2 secondi
    private IEnumerator ShowImageForDuration(Image img)
    {
        img.enabled = true;  // Mostra l'immagine
        yield return new WaitForSeconds(2f);  // Aspetta 2 secondi
        img.enabled = false;  // Nascondi l'immagine
    }

    private void UpdatePath()
    {
        if (movementController != null)
        {
            movementController.NotifyNewStep(highestStepReached);
        }
    }

    //private void UpdateImage()
    //{
    //    if (displayImage != null && highestStepReached < images.Count - 1)
    //    {
    //        displayImage.sprite = images[highestStepReached + 1]; 
    //    } else
    //    {
    //        StartCoroutine(PlayStepAudioWithDelay(true));
    //        endScreen.SetActive(true);
    //        Debug.Log("Congratulazioni" + highestStepReached);
    //    }
    //}

    //public void RepeteStep()
    //{
    //    if (audioSource != null && (highestStepReached + 1) < audioList.Count)
    //    {
    //        audioSource.clip = audioList[highestStepReached + 1];
    //        audioSource.Play();
    //    }
    //}

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
        //if (audioSource != null && (highestStepReached + 1) < audioList.Count)
        //{
        //    audioSource.clip = audioList[highestStepReached + 1];
        //    audioSource.Play();
        //}
    }

    public void notifyStateProcessingTime(float processingTime)
    {
        processingTime = Mathf.Clamp(processingTime, 0f, 10f);
        float remainingTime = 10f - processingTime;

        string formattedTime = $"00:{remainingTime:00}";
        timeText.text = formattedTime;

        // Attiva il timer screen
        timerScreen.SetActive(true);

        // Reset della coroutine se sta gi� contando per spegnere lo schermo
        if (hideTimerCoroutine != null)
        {
            StopCoroutine(hideTimerCoroutine);
        }
        hideTimerCoroutine = StartCoroutine(HideTimerAfterDelay());
    }

    private IEnumerator HideTimerAfterDelay()
    {
        yield return new WaitForSeconds(hideDelay);
        timerScreen.SetActive(false);
        hideTimerCoroutine = null;
    }
}
