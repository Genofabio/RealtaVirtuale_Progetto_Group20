using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class OvenStartButton : MonoBehaviour, Button
{
    [SerializeField] private Oven oven;

    [SerializeField] private List<AudioClip> audioList;
    private AudioSource audioSource;

    private Vector3 originalPosition;
    [SerializeField] private Vector3 pressedOffset = new Vector3(0f, -0.05f, -0.05f);
    private float pressDuration = 0.1f;  

    

    private void Start()
    {
        originalPosition = transform.position;
        audioSource = GetComponent<AudioSource>();

        // Debug per verificare che l'AudioSource sia presente
        if (audioSource == null)
        {
            Debug.LogError("AudioSource non trovato! Aggiungilo all'oggetto.");
        }
    }

    public void ToggleCooking()
    {

        StartCoroutine(PressButton());

        if (audioList != null && audioList.Count > 0 && audioSource != null)
        {
            audioSource.PlayOneShot(audioList[0]);
        }

        Debug.Log("Pulsante premuto");

        oven.ToggleCooking(); 
    }

    private System.Collections.IEnumerator PressButton()
    {
        transform.position = originalPosition + pressedOffset;

        yield return new WaitForSeconds(pressDuration);

        transform.position = originalPosition;
    }

    public void Press()
    {
        Debug.Log("Press fatto");
        ToggleCooking();
    }
}
