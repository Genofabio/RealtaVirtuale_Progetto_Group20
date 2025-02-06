using UnityEngine;

public class RemoveFilterTrigger : MonoBehaviour    
{
    [SerializeField] Becher _becher;
    //private Filter _filter = null;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enter" + other);
        if (other.TryGetComponent<PaperFilter>(out var filter))
        {
            Debug.Log("Filtro sul becher");
            //_filter = filter;
            filter.ApplyFilter(_becher);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exit" + other);
        if (other.TryGetComponent<PaperFilter>(out var filter))
        {
            Debug.Log("Filtro allontanato...");
            filter.RemoveFilter();
            //_filter = null;
        }
    }
}
