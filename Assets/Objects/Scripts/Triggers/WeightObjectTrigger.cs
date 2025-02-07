using UnityEngine;

public class WeightObjectTrigger : MonoBehaviour
{
    [SerializeField] private PrecisionBalance _balance;
    //private float defaultObjectWeight = 10f;  //da rimuovere quando verranno aggiunti i pesi agli oggetti
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Rigidbody>(out var rigidBody))
        {
            _balance.AddWeight(rigidBody.mass);
        }
        //Debug.Log("Poggiato oggetto sulla bilancia: +" + rigidBody.mass);
        //GameObject obj = other.gameObject;          servirà poi prendere il peso dell'oggetto
        
    }

    //public void OnTriggerStay(Collider other)
    //{
    //    Debug.Log("Oggetto in bilancia: " + other.gameObject.name);
    //}

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("Rimosso oggetto dalla bilancia: " + other.gameObject.name);
        if (other.TryGetComponent<Rigidbody>(out var rigidBody))
        {
            _balance.RemoveWeight(rigidBody.mass);
        }
        //Debug.Log("Rimosso oggetto dalla bilancia: -" + rigidBody.mass);
    }
}
