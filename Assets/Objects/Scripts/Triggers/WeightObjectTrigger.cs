using UnityEngine;

public class WeightObjectTrigger : MonoBehaviour
{
    [SerializeField] private PrecisionBalance _balance;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Rigidbody>(out var rigidBody))
        {
            _balance.AddWeight(rigidBody);
        }
        Debug.Log("Poggiato oggetto sulla bilancia: +" + other.name);
        //GameObject obj = other.gameObject;          servirà poi prendere il peso dell'oggetto

    }

    public void OnTriggerStay(Collider other)
    {
        _balance.WeightObjects();
        //Debug.Log("Oggetto in bilancia: " + other.gameObject.name);
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("Rimosso oggetto dalla bilancia: " + other.gameObject.name);
        if (other.TryGetComponent<Rigidbody>(out var rigidBody))
        {
            _balance.RemoveWeight(rigidBody);
        }
        Debug.Log("Rimosso oggetto dalla bilancia: -" + other.name);
    }
}
