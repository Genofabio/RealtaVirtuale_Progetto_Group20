using UnityEngine;
using System.Collections.Generic;

public class BalanceButton : MonoBehaviour
{
    public PrecisionBalance precisionBalance;
    private float tare { get; set; } = 0f;

    private Vector3 originalPosition;  
    public Vector3 pressedOffset = new Vector3(0f, -0.05f, -0.05f); 
    public float pressDuration = 0.1f;  

    [SerializeField] private List<AudioClip> audioList; // Lista di suoni
    private AudioSource audioSource;

    private void Start()
    {
        originalPosition = transform.position;
        audioSource = GetComponent<AudioSource>(); // Prende il componente AudioSource

        // Debug per verificare che l'AudioSource sia presente
        if (audioSource == null)
        {
            Debug.LogError("AudioSource non trovato! Aggiungilo all'oggetto.");
        }
    }

    public void Tare()
    {
        StartCoroutine(PressButton());

        tare = -precisionBalance.GetTotalWeight();
        precisionBalance.SetCurrentTare(tare);

        Debug.Log("Taratura bilancia effettuata, peso totale: " + RoundToDecimalPlaces(precisionBalance.GetTotalWeight() + tare, 2));

        // Debug per vedere se l'AudioSource è stato trovato correttamente
        if (audioSource == null)
        {
            Debug.LogError("AudioSource non trovato!");
        }
        else
        {
            Debug.Log("AudioSource trovato!");
        }

        // Verifica se audioList contiene dei clip
        if (audioList != null && audioList.Count > 0)
        {
            audioSource.clip = audioList[0]; // Suono taratura
            audioSource.Play();
        }
        else
        {
            Debug.LogError("audioList è vuota o non è inizializzata correttamente.");
        }
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
}
