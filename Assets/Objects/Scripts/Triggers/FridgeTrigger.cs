using UnityEngine;

public class FridgeTrigger : MonoBehaviour
{
    [SerializeField] private Fridge fridge;

    private void Start()
    {
        fridge = GetComponentInParent<Fridge>();
    }

    private void OnTriggerEnter(Collider other)
    {
        fridge.IsEmpty = false;
    }

    private void OnTriggerExit(Collider other)
    {
        fridge.IsEmpty = true;
    }
}
