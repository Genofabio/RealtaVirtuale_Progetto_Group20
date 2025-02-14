using System.Collections;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    [SerializeField] private GameObject intact;
    [SerializeField] private GameObject convexHull;
    [SerializeField] private GameObject broken;

    [SerializeField] private float breakForceThreshold = 5f;
    [SerializeField] private float destructionDelay = 5f;

    private Collider objCollider;
    private Rigidbody brokenRigidbody;

    private AudioSource audioSource;  // Aggiungi una variabile per l'AudioSource
    private AudioClip breakSound;    // Aggiungi una variabile per il suono di rottura

    private void Awake()
    {
        intact.SetActive(true);
        broken.SetActive(false);

        objCollider = GetComponent<Collider>();
        brokenRigidbody = broken.GetComponent<Rigidbody>();

        // Aggiungi il componente AudioSource se non esiste
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Carica il suono di rottura dalla cartella Resources
        breakSound = Resources.Load<AudioClip>("Sounds/BreakSound"); // Assicurati che il nome del file audio sia corretto
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Misura la forza dell'impatto
        float impactForce = collision.relativeVelocity.magnitude;

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

        // Riproduci il suono di rottura
        if (breakSound != null)
        {
            audioSource.PlayOneShot(breakSound);
        }

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
