using UnityEngine;
using System.Collections;

public class StirringRod : MonoBehaviour
{
    [SerializeField] private float tiltAngle = 10f; // Angolo di inclinazione sull'asse X
    [SerializeField] private float rotationSpeed = 360f; // Gradi al secondo per la rotazione attorno a Y
    [SerializeField] private float mixDuration = 1f; // Durata della rotazione
    [SerializeField] private float moveDuration = 0.001f; // Durata dello spostamento nella nuova posizione

    private Rigidbody rb;
    private Collider col;

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Ottiene il riferimento al Rigidbody
        col = GetComponent<Collider>(); // Ottiene il riferimento al Collider
    }

    public void Mix(Fillable contenitor, Vector3 targetPosition)
    {
        contenitor.StirContents();
        StartCoroutine(MoveAndStir(targetPosition));
    }

    private IEnumerator MoveAndStir(Vector3 targetPosition)
    {
        // Disabilita Rigidbody e Collider
        if (rb != null) rb.isKinematic = true;
        if (col != null) col.enabled = false;

        // Salva la posizione originale
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        // Spostamento graduale verso la posizione target
        while (elapsedTime < moveDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition; // Assicura che arrivi esattamente al punto target

        // Avvia la rotazione
        yield return StartCoroutine(AnimateStirring());

        // Torna alla posizione originale
        elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            transform.position = Vector3.Lerp(targetPosition, startPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = startPosition; // Assicura il ritorno esatto

        // Riabilita Rigidbody e Collider
        if (rb != null) rb.isKinematic = false;
        if (col != null) col.enabled = true;
    }

    private IEnumerator AnimateStirring()
    {
        float elapsedTime = 0f;
        Quaternion initialRotation = transform.rotation;
        Quaternion tiltedRotation = Quaternion.Euler(tiltAngle, 0, 0) * initialRotation;

        while (elapsedTime < mixDuration)
        {
            float progress = elapsedTime / mixDuration;
            float currentRotationY = progress * rotationSpeed * 2; // Rotazione progressiva di 360°

            // Ruota attorno all'asse Y del mondo, mantenendo l'inclinazione X
            Quaternion rotationY = Quaternion.AngleAxis(currentRotationY, Vector3.up);
            transform.rotation = rotationY * tiltedRotation;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = initialRotation; // Ripristina la rotazione originale
    }
}