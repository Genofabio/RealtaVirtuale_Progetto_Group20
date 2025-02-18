using System.Collections;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    [SerializeField] private GameObject intact;
    [SerializeField] private GameObject convexHull;
    [SerializeField] private GameObject broken;

    [SerializeField] private float breakForceThreshold = 5f;
    [SerializeField] private float grabbedBreakForceThreshold = 5f;

    [SerializeField] private float destructionDelay = 5f;

    [SerializeField] private AudioClip breakSound; // Campo per il suono di rottura configurabile nell'Inspector
    private AudioSource audioSource;  // AudioSource per riprodurre il suono

    private Collider objCollider;
    private Rigidbody brokenRigidbody;
    private Grabbable grabbableComponent;

    private void Awake()
    {
        intact.SetActive(true);
        broken.SetActive(false);

        objCollider = GetComponent<Collider>();
        brokenRigidbody = broken.GetComponent<Rigidbody>();

        // Aggiungi un AudioSource se non è già presente
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (TryGetComponent<Grabbable>(out grabbableComponent));

    }

    private void OnCollisionEnter(Collision collision)
    {
        // Misura la forza dell'impatto
        float impactForce = collision.relativeVelocity.magnitude;

        if (grabbableComponent != null)
        {
            if (grabbableComponent.IsGrabbed() && impactForce >= grabbedBreakForceThreshold)
            {
                Break();
            } 
            else if (!grabbableComponent.IsGrabbed() && impactForce >= breakForceThreshold)
            {
                Break();
            }
        } 
        else
        {
            if (impactForce >= breakForceThreshold)
            {
                Break();
            }
        }
    }

    private void Break()
    {
        // Riproduci il suono di rottura
        if (breakSound != null)
        {
            audioSource.PlayOneShot(breakSound);
        }

        // Disattiva solo la mesh e il collider di "intact" invece di disattivare l'intero GameObject
        if (intact.TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer))
        {
            meshRenderer.enabled = false;
        }
        if (intact.TryGetComponent<Collider>(out Collider collider))
        {
            collider.enabled = false;
        }
        if (intact.TryGetComponent<Grabbable>(out Grabbable grabbable))
        {
            grabbable.Drop();
        }

        if (convexHull != null)
        {
            foreach (Collider hullCollider in convexHull.GetComponentsInChildren<Collider>())
            {
                hullCollider.enabled = false;
            }
        }

        foreach (Transform child in intact.transform)
        {
            if (child.GetComponent<SolidRenderer>() || child.GetComponent<LiquidRenderer>())
            {
                if (child.TryGetComponent<MeshRenderer>(out MeshRenderer childMeshRenderer))
                {
                    childMeshRenderer.enabled = false;
                }
            }
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

        // Aspetta per il tempo definito (5 secondi)
        yield return new WaitForSeconds(destructionDelay);

        // Distrugge l'oggetto dopo il ritardo
        Destroy(gameObject);
    }
}
