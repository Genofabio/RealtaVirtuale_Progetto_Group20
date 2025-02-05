using UnityEngine;

public class WeightObjectTrigger : MonoBehaviour
{
    [SerializeField] private PrecisionBalance _balance;
    private float defaultObjectWeight = 10f;  //da rimuovere quando verranno aggiunti i pesi agli oggetti
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Poggiato oggetto sulla bilancia: " + other.gameObject.name);
        //GameObject obj = other.gameObject;          servirà poi prendere il peso dell'oggetto
        _balance.AddWeight(defaultObjectWeight);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Rimosso oggetto dalla bilancia: " + other.gameObject.name);
        //GameObject obj = other.gameObject;          servirà poi prendere il peso dell'oggetto
        _balance.RemoveWeight(defaultObjectWeight);
    }
}
