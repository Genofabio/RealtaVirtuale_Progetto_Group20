using UnityEngine;

public class OvenTrigger : MonoBehaviour
{
    [SerializeField] private Oven oven;

    private void Start()
    {
        oven = GetComponentInParent<Oven>();
    }    

    private void OnTriggerEnter(Collider other)
    {
        oven.IsEmpty = false;
    }
    
    private void OnTriggerExit(Collider other)
    {
        oven.IsEmpty = true;
    }
}
