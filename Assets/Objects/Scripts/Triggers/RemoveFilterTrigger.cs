using UnityEngine;

public class RemoveFilterTrigger : MonoBehaviour    
{
    [SerializeField] Becher _becher;
    private Filter _filter = null;
    [SerializeField] bool _removeFilterOnExit = true;
    [SerializeField] bool _attachFilterOnEnter = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Filter>(out var filter))
        {
            Debug.Log("Filtro allontanato dal becher");
            filter.RemoveFilter();
            _filter = null;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent<Filter>(out var filter))
        {
            Debug.Log("filtro sul becher");
            _filter = filter;
        }
    }
}
