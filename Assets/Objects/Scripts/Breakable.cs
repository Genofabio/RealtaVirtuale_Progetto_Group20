using System.Collections;
using UnityEngine;

public class Breakable : MonoBehaviour
{

    [SerializeField] private GameObject intact;
    [SerializeField] private GameObject broken;

    [SerializeField] private float breakForceThreshold = 5f;
    [SerializeField] private float destructionDelay = 5f;

    private Collider objCollider;
    private Rigidbody brokenRigidbody;

    private void Awake()
    {
        intact.SetActive(true);
        broken.SetActive(false);

        objCollider = GetComponent<Collider>();
        brokenRigidbody = broken.GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Misura la forza dell'impatto
        float impactForce = collision.relativeVelocity.magnitude;
        Debug.Log("Impact: " + impactForce);

        if (impactForce >= breakForceThreshold)
        {
            Break();
        }
    }

    private void Break()
    {
        // Disattiva solo la mesh e il collider di "intact" invece di disattivare l'intero GameObject
        if (intact.TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer))
        {
            meshRenderer.enabled = false;
        }
        if (intact.TryGetComponent<Collider>(out Collider collider))
        {
            collider.enabled = false;
        }

        // Attiva il modello rotto
        broken.SetActive(true);

        // Avvia la dissolvenza e poi distrugge l'oggetto
        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        if (TryGetComponent<Grabbable>(out Grabbable grab))
        {
            grab.DeleteLineRenderer();
        }

        // Aspetta per il tempo definito (10 secondi)
        yield return new WaitForSeconds(destructionDelay);

        // Distrugge l'oggetto dopo il ritardo
        Destroy(gameObject);
    }
}
