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

        Fillable fillable = other.GetComponent<Fillable>();
        if (fillable != null && fillable.GetContainedSubstanceMixture() != null)
        {
            fridge.InsertIntoFridge(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        fridge.IsEmpty = true;

        Fillable fillable = other.GetComponent<Fillable>();
        if (fillable != null && fillable.GetContainedSubstanceMixture() != null)
        {
            fridge.RemoveFromFridge(other.gameObject);
        }
    }
}
