using UnityEngine;
using System.Collections;

public class SlidingDoor : MonoBehaviour, Openable
{
    private Vector3 closedPosition;
    private Vector3 openPosition;
    private BoxCollider boxCollider;

    [SerializeField] private float slideDistance = 5f; // Distanza di scorrimento (personalizzabile)
    [SerializeField] private Vector3 direction = Vector3.right; // Direzione di scorrimento (es. destra o sinistra)
    [SerializeField] private float moveSpeed = 2f; // Velocità di movimento della porta

    public bool IsOpen { get; protected set; } = false;
    public bool IsMoving { get; protected set; } = false;

    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        closedPosition = transform.localPosition;
        openPosition = closedPosition + direction.normalized * slideDistance; // Calcola la posizione aperta
    }

    public void ToggleDoor()
    {
        if (IsMoving) return;

        if (IsOpen)
            StartCoroutine(CloseDoorCoroutine());
        else
            StartCoroutine(OpenDoorCoroutine());
    }

    protected virtual IEnumerator OpenDoorCoroutine()
    {
        IsMoving = true;
        IsOpen = true;
        if (boxCollider) boxCollider.enabled = false; // Disattiva il BoxCollider
        yield return MoveDoor(openPosition); // Muove la porta verso la posizione aperta
        if (boxCollider) boxCollider.enabled = true; // Riattiva il BoxCollider
        IsMoving = false;
    }

    protected virtual IEnumerator CloseDoorCoroutine()
    {
        IsMoving = true;
        if (boxCollider) boxCollider.enabled = false; // Disattiva il BoxCollider
        yield return MoveDoor(closedPosition); // Muove la porta verso la posizione chiusa
        if (boxCollider) boxCollider.enabled = true; // Riattiva il BoxCollider
        IsOpen = false;
        IsMoving = false;
    }

    private IEnumerator MoveDoor(Vector3 targetPosition)
    {
        float duration = Vector3.Distance(transform.localPosition, targetPosition) / moveSpeed; // Calcola il tempo di movimento in base alla velocità
        float elapsed = 0f;

        Vector3 startPosition = transform.localPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t); // Sposta la porta
            yield return null;
        }

        transform.localPosition = targetPosition; // Assicurati che la porta arrivi esattamente alla posizione finale
    }
}
