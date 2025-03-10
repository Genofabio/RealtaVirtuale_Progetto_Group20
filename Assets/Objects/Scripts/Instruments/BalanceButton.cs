using UnityEngine;
using System.Collections.Generic;

public class BalanceButton : MonoBehaviour, Button
{
    public PrecisionBalance precisionBalance;
    private float tare { get; set; } = 0f;

    private Vector3 originalPosition;  
    public Vector3 pressedOffset = new Vector3(0f, -0.05f, -0.05f); 
    public float pressDuration = 0.1f;  

    [SerializeField] private List<AudioClip> audioList;
    private AudioSource audioSource;

    private bool isTaring = false;

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

    public void Tare()
    {

        StartCoroutine(PressButton());

        if (audioList != null && audioList.Count > 0 && audioSource != null)
        {
            audioSource.PlayOneShot(audioList[0]);
        }

        if (isTaring) return;

        isTaring = true;
        Invoke(nameof(ResetTareCooldown), 0.5f);

        tare = -precisionBalance.GetTotalWeight();
        precisionBalance.SetCurrentTare(tare);

        
    }

    private void ResetTareCooldown()
    {
        isTaring = false;
    }

    private System.Collections.IEnumerator PressButton()
    {
        transform.position = originalPosition + pressedOffset;

        yield return new WaitForSeconds(pressDuration);

        transform.position = originalPosition;
    }

    private float RoundToDecimalPlaces(float value, int decimalPlaces)
    {
        float factor = Mathf.Pow(10, decimalPlaces);
        return Mathf.Round(value * factor) / factor;
    }

    public void Press()
    {
        Tare();
    }
}
