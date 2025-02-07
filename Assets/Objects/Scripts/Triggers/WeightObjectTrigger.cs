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
    }

    public void OnTriggerStay(Collider other)
    {
        _balance.WeightObjects();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Rigidbody>(out var rigidBody))
        {
            _balance.RemoveWeight(rigidBody);
        }
    }
}
