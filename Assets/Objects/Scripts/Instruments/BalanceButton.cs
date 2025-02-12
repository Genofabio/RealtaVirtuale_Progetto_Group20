using UnityEngine;

public class BalanceButton : MonoBehaviour
{
    public PrecisionBalance precisionBalance;
    private float tare { get; set; } = 0f;

    private Vector3 originalPosition;  
    public Vector3 pressedOffset = new Vector3(0f, -0.05f, -0.05f); 
    public float pressDuration = 0.1f;  

    private void Start()
    {
        originalPosition = transform.position;
    }

    public void Tare()
    {
        StartCoroutine(PressButton());

        tare = -precisionBalance.GetTotalWeight();
        precisionBalance.SetCurrentTare(tare);

        Debug.Log("Taratura bilancia effettuata, peso totale: " + RoundToDecimalPlaces(precisionBalance.GetTotalWeight() + tare, 2));
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